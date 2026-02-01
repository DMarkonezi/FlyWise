using Microsoft.AspNetCore.Mvc;
using Api.models.neo4j.Nodes;
using Api.Repositories;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CityController : ControllerBase
{
    private readonly CityRepository _repo;
    public CityController(CityRepository repo) => _repo = repo;

    [HttpPost("{countryId}")]
    public async Task<IActionResult> Create(int countryId, [FromBody] City city)
    {
        // Ne moraš slati Id u JSON-u, biće generisan automatski
        await _repo.CreateAsync(city, countryId);
        return Ok($"Grad {city.Name} je uspešno kreiran i povezan sa državom.");
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var cities = await _repo.GetAllAsync();
        return Ok(cities);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var city = await _repo.GetByIdAsync(id);
        return city != null ? Ok(city) : NotFound();
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] City city)
    {
        await _repo.UpdateAsync(city);
        return Ok("Grad je ažuriran.");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _repo.DeleteAsync(id);
        return Ok("Grad je obrisan.");
    }
}