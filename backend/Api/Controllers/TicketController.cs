using Microsoft.AspNetCore.Mvc;
using Api.models.neo4j.Nodes;
using Api.Repositories;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TicketController : ControllerBase
{
    private readonly TicketRepository _repo;
    public TicketController(TicketRepository repo) => _repo = repo;

    [HttpPost("{passengerId}/{routeId}")]
    public async Task<IActionResult> Create(int passengerId, int routeId, [FromBody] Ticket ticket)
    {
        await _repo.CreateAsync(ticket, passengerId, routeId);
        return Ok("Karta kreirana.");
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _repo.GetAllAsync());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var ticket = await _repo.GetByIdAsync(id);
        return ticket != null ? Ok(ticket) : NotFound();
    }

    [HttpGet("detailed")]
    public async Task<IActionResult> GetDetailed()
    {
        return Ok(await _repo.GetAllDetailedAsync());
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] Ticket ticket)
    {
        await _repo.UpdateAsync(ticket);
        return Ok("Karta ažurirana.");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _repo.DeleteAsync(id);
        return Ok("Karta obrisana.");
    }
}