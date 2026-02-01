using Microsoft.AspNetCore.Mvc;
using Api.models.neo4j.Nodes;
using Api.Repositories;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FlightController : ControllerBase
{
    private readonly FlightRepository _repo;

    public FlightController(FlightRepository repo)
    {
        _repo = repo;
    }

    // CREATE: api/Flight/{routeId}/{order}
    [HttpPost("{routeId}/{order}")]
    public async Task<IActionResult> Create(int routeId, int order, [FromBody] Flight flight)
    {
        try
        {
            await _repo.CreateAsync(flight, routeId, order);
            return Ok($"Let {flight.FlightNumber} je uspešno kreiran.");
        }
        catch (Exception ex)
        {
            return BadRequest($"Greška pri kreiranju leta: {ex.Message}");
        }
    }

    // GET ALL: api/Flight
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var flights = await _repo.GetAllAsync();
        return Ok(flights);
    }

    // GET DETAILED: api/Flight/detailed
    [HttpGet("detailed")]
    public async Task<IActionResult> GetAllDetailed()
    {
        var detailedFlights = await _repo.GetAllDetailedAsync();
        return Ok(detailedFlights);
    }

    // GET BY ID: api/Flight/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var flight = await _repo.GetByIdAsync(id);
        if (flight == null) return NotFound("Let nije pronađen.");
        return Ok(flight);
    }

    // UPDATE: api/Flight
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] Flight flight)
    {
        try
        {
            await _repo.UpdateAsync(flight);
            return Ok("Podaci o letu su ažurirani.");
        }
        catch (Exception ex)
        {
            return BadRequest($"Greška pri ažuriranju: {ex.Message}");
        }
    }

    // DELETE: api/Flight/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _repo.DeleteAsync(id);
            return Ok("Let je uspešno obrisan.");
        }
        catch (Exception ex)
        {
            return BadRequest($"Greška pri brisanju: {ex.Message}");
        }
    }
}