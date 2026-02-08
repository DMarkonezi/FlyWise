using Microsoft.AspNetCore.Mvc;
using Api.models.neo4j.Nodes;
using Api.Repositories;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BillController : ControllerBase
{
    private readonly BillRepository _repo;
    public BillController(BillRepository repo) => _repo = repo;

    [HttpPost("{ticketId}")]
    public async Task<IActionResult> Create(int ticketId, [FromBody] Bill bill)
    {
        try 
        {
            await _repo.CreateBillAsync(bill, ticketId);
            return Ok("Račun je uspešno generisan i povezan sa kartom.");
        }
        catch (Exception ex)
        {
            return BadRequest($"Greška pri kreiranju računa: {ex.Message}");
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var bill = await _repo.GetBillByIdAsync(id);
        return bill != null ? Ok(bill) : NotFound("Račun nije pronađen.");
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] Bill bill)
    {
        await _repo.UpdateBillAsync(bill);
        return Ok("Podaci na računu su ažurirani.");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _repo.DeleteBillAsync(id);
        return Ok("Račun je uspešno obrisan iz baze.");
    }

    [HttpGet("by-ticket/{ticketId}")]
    public async Task<IActionResult> GetByTicketId(int ticketId)
    {
        var bill = await _repo.GetBillByTicketIdAsync(ticketId);
        return bill != null ? Ok(bill) : NotFound("Račun za ovu kartu ne postoji.");
    }
}