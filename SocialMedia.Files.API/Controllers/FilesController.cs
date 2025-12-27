using Files.EntityFrameworkCore.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialMedia.Files.API.Data;
using SocialMedia.Files.API.Services;
using System.Diagnostics;

namespace SocialMedia.Files.API.Controllers;

[ApiController]
[Route("api/{shardKey}/files")]
public class FilesController : ControllerBase
{
    private readonly IFileService _fileService;
    private readonly ILogger<FilesController> _logger;

    public FilesController(IFileService fileService, ILogger<FilesController> logger)
    {
        _fileService = fileService;
        _logger = logger;
    }

    private string? GetUserId()
    {
        // Adjust claim type based on your token structure
        // Often it's ClaimTypes.NameIdentifier or "sub"
        return User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value 
               ?? User.FindFirst("sub")?.Value;
    }

    [HttpPost]
    [Authorize]
    [DisableRequestSizeLimit]
    public async Task<IActionResult> Upload(string shardKey, IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized("User ID not found in token");

        Activity.Current?.AddTag("file.name", file.FileName);
        Activity.Current?.AddTag("file.size", file.Length);
        Activity.Current?.AddTag("file.content_type", file.ContentType);

        var (userFileId, url) = await _fileService.UploadFileAsync(shardKey, userId, file);

        return Ok(new { id = userFileId, url });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Download(string shardKey, Guid id)
    {
        // For download, we might verify auth inside service or here.
        // If we want to support public downloads (if logic allows), userId can be null.
        // But the service logic enforced ownership if userId is present, or threw if not owned.
        // If the endpoint is public (no [Authorize]), User might be unauthenticated.
        var userId = GetUserId();

        try 
        {
            var (stream, contentType, fileName) = await _fileService.DownloadFileAsync(shardKey, userId, id);
            
            Activity.Current?.AddTag("file.download_id", id.ToString());
            
            return File(stream, contentType, fileName);
        }
        catch (FileNotFoundException)
        {
            return NotFound();
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid(); // Or Unauthorized
        }
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(string shardKey, Guid id)
    {
        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        try
        {
            await _fileService.DeleteFileAsync(shardKey, userId, id);
            return NoContent();
        }
        catch (UnauthorizedAccessException)
        {
             return Forbid();
        }
    }
}
