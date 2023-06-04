using Microsoft.AspNetCore.Mvc;
using Pagination.EntityFrameworkCore.Extensions;
using SocialMediaApi.Domain.Entities;
using SocialMediaApi.Repositories;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SocialMediaApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupsController : ControllerBase
    {
        private readonly SocialMediaApiDbContext _dbContext;

        public GroupsController(SocialMediaApiDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: api/<GroupsController>
        [HttpGet]
        public async Task<ActionResult<Pagination<Group>>> Get()
        {
            var tt = await _dbContext.AsPaginationAsync<Group>(1, 10);
            return Ok(tt);
        }

        // GET api/<GroupsController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<GroupsController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<GroupsController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<GroupsController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}