using Microsoft.AspNetCore.Mvc;
using Api.models.neo4j.Nodes;
using Api.Repositories;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SuitcasesController : ControllerBase
{
    private readonly SuitcasesRepository _repo;
    public SuitcasesController(SuitcasesRepository repo) => _repo = repo;

    [HttpPost("{ticketId}/{count}")]
    public async Task<IActionResult> Create(int ticketId, int count, [FromBody] Suitcases suitcases)
    {
        await _repo.CreateAsync(suitcases, ticketId, count);
        return Ok("Prtljag uspešno registrovan za kartu.");
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _repo.GetAllAsync());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _repo.GetByIdAsync(id);
        return result != null ? Ok(result) : NotFound();
    }

    [HttpGet("detailed")]
    public async Task<IActionResult> GetDetailed()
    {
        return Ok(await _repo.GetDetailedAsync());
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] Suitcases suitcases)
    {
        await _repo.UpdateAsync(suitcases);
        return Ok("Dozvoljena težina prtljaga ažurirana.");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _repo.DeleteAsync(id);
        return Ok("Prtljag obrisan.");
    }
}