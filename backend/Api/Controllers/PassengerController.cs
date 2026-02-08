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

    // CREATE passenger for specific user
    [HttpPost("Create/{userId}")]
    public async Task<IActionResult> Create(int userId, [FromBody] Passenger passenger)
    {
        await _repo.CreateAsync(passenger, userId);
        return Ok($"Passenger {passenger.FirstName} {passenger.LastName} created.");
    }

    // GET all passengers
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _repo.GetAllAsync());
    }

    // GET passenger by ID
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var passenger = await _repo.GetByIdAsync(id);
        return passenger != null
            ? Ok(passenger)
            : NotFound("Passenger not found.");
    }

    // GET all passengers for one user
    [HttpGet("ByUser/{userId}")]
    public async Task<IActionResult> GetByUser(int userId)
    {
        return Ok(await _repo.GetByUserAsync(userId));
    }

    // UPDATE passenger
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] Passenger passenger)
    {
        await _repo.UpdateAsync(passenger);
        return Ok("Passenger updated.");
    }

    // DELETE passenger
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _repo.DeleteAsync(id);
        return Ok("Passenger deleted.");
    }
}
