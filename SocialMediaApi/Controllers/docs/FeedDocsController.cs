using Microsoft.AspNetCore.Mvc;
using PlantUml.Net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SocialMediaApi.Controllers.docs
{
    [Route("api/docs/v1/feed")]
    [ApiController]
    public class FeedDocsController : ControllerBase
    {
        // GET: api/<FeedController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var factory = new RendererFactory();

            var renderer = factory.CreateRenderer(new PlantUmlSettings());

            var bytes = await renderer.RenderAsync("Bob -> Alice : Hello", OutputFormat.Png);
            return File(bytes, "image/png");
        }
    }
}