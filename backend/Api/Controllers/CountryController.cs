using Microsoft.AspNetCore.Mvc;
using Api.models.neo4j.Nodes;
using Api.Repositories;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CountryController : ControllerBase
{
    private readonly CountryRepository _repo;
    public CountryController(CountryRepository repo) => _repo = repo;

   [HttpPost]
public async Task<IActionResult> Create([FromBody] Country country)
{
    // Ako je Id tipa int, ovo je malo teže bez brojanja čvorova, 
    // ali ako promeniš Id u string/Guid:
    // country.Id = Guid.NewGuid().ToString(); 
    
    await _repo.CreateAsync(country);
    return Ok();
}

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _repo.GetAllAsync());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var country = await _repo.GetByIdAsync(id);
        return country != null ? Ok(country) : NotFound();
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] Country country)
    {
        await _repo.UpdateAsync(country);
        return Ok("Država je ažurirana.");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _repo.DeleteAsync(id);
        return Ok("Država je obrisana.");
    }
}