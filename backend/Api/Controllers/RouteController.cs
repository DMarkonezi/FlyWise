using Microsoft.AspNetCore.Mvc;
using Api.models.neo4j.Nodes;
using Api.Repositories;
using Route = Api.models.neo4j.Nodes.Route;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RouteController : ControllerBase
{
    private readonly RouteRepository _repo;
    public RouteController(RouteRepository repo) => _repo = repo;

    [HttpPost("{fromCityId}/{toCityId}")]
    public async Task<IActionResult> Create(int fromCityId, int toCityId, [FromBody] Route route)
    {
        try
        {
            await _repo.CreateAsync(route, fromCityId, toCityId);
            return Ok("Ruta je uspešno kreirana i povezana sa gradovima.");
        }
        catch (Exception ex)
        {
            return BadRequest($"Greška: {ex.Message}");
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _repo.GetAllDetailedAsync());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var route = await _repo.GetByIdAsync(id);
        return route != null ? Ok(route) : NotFound();
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] Route route)
    {
        await _repo.UpdateAsync(route);
        return Ok("Ruta je ažurirana.");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _repo.DeleteAsync(id);
        return Ok("Ruta je obrisana.");
    }
}