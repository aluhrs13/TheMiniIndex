using Microsoft.AspNetCore.Mvc;

namespace MiniIndex.API;
[Route("api/[controller]")]
[ApiController]
public class TagsController : ControllerBase
{
    // GET: api/<TagsController>
    [HttpGet]
    public IEnumerable<string> Get()
    {
        return new string[] { "value1", "value2" };
    }

    // GET api/<TagsController>/5
    [HttpGet("{id}")]
    public string Get(int id)
    {
        return "value";
    }

    // POST api/<TagsController>
    [HttpPost]
    public void Post([FromBody] string value)
    {
    }

    // PUT api/<TagsController>/5
    [HttpPut("{id}")]
    public void Put(int id, [FromBody] string value)
    {
    }

    // DELETE api/<TagsController>/5
    [HttpDelete("{id}")]
    public void Delete(int id)
    {
    }
}
