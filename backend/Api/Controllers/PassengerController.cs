using Microsoft.AspNetCore.Mvc;
using Api.models.neo4j.Nodes;
using Api.Repositories;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PassengerController : ControllerBase
{
    private readonly PassengerRepository _repo;

    public PassengerController(PassengerRepository repo)
    {
        _repo = repo;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Passenger passenger)
    {
        await _repo.CreateAsync(passenger);
        return Ok($"Putnik {passenger.FirstName} {passenger.LastName} je uspešno kreiran.");
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _repo.GetAllAsync());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetDetailed(int id)
    {
        var data = await _repo.GetDetailedAsync(id);
        return data != null ? Ok(data) : NotFound("Putnik nije pronađen.");
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] Passenger passenger)
    {
        await _repo.UpdateAsync(passenger);
        return Ok("Podaci putnika su ažurirani.");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _repo.DeleteAsync(id);
        return Ok("Putnik je obrisan.");
    }
}