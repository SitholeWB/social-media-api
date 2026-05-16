using Microsoft.EntityFrameworkCore;
using SocialMedia.Domain;
using Microsoft.AspNetCore.Authorization;

namespace SocialMedia.API;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/tenants")]
[ApiController]
public class TenantsController : ControllerBase
{
    private readonly IDispatcher _dispatcher;

    public TenantsController(IDispatcher _dispatcher)
    {
        this._dispatcher = _dispatcher;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetTenants(CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync<GetTenantsQuery, List<TenantDto>>(new GetTenantsQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> CreateTenant([FromBody] CreateTenantCommand command, CancellationToken cancellationToken)
    {
        var tenantId = await _dispatcher.SendAsync<CreateTenantCommand, Guid>(command, cancellationToken);
        return CreatedAtAction(nameof(GetTenants), new { id = tenantId }, new { Id = tenantId });
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> UpdateTenant(Guid id, [FromBody] UpdateTenantCommand command, CancellationToken cancellationToken)
    {
        if (id != command.Id) return BadRequest();
        await _dispatcher.SendAsync<UpdateTenantCommand, bool>(command, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> DeleteTenant(Guid id, CancellationToken cancellationToken)
    {
        await _dispatcher.SendAsync<DeleteTenantCommand, bool>(new DeleteTenantCommand(id), cancellationToken);
        return NoContent();
    }
}
