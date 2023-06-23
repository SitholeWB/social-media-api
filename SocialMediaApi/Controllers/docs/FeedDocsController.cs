using Microsoft.AspNetCore.Mvc;
using PlantUml.Net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SocialMediaApi.Controllers.docs
{
    [Route("api/v1/docs/feed")]
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

        // GET api/<FeedController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<FeedController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<FeedController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<FeedController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}