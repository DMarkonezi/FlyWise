using Microsoft.AspNetCore.Mvc;
using Api.models.neo4j.Nodes;
using Api.Repositories;
using Api.Models.Auth;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly UserRepository _repo;

    public UserController(UserRepository repo)
    {
        _repo = repo;
    }

    [HttpPost("Create")]
    public async Task<IActionResult> Create([FromBody] User user)
    {
        await _repo.CreateAsync(user);
        return Ok($"Putnik {user.FirstName} {user.LastName} je uspešno kreiran.");
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
    public async Task<IActionResult> Update([FromBody] User user)
    {
        await _repo.UpdateAsync(user);
        return Ok("Podaci putnika su ažurirani.");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _repo.DeleteAsync(id);
        return Ok("Putnik je obrisan.");
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        // hash-ovanje lozinke
        string hashedPassword = HashPassword(dto.Password);

        var user = new User
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            PasswordHash = hashedPassword,
            PassportNumber = dto.PassportNumber,
            CreatedAt = DateTime.Now
        };

        await _repo.CreateAsync(user);
        return Ok(new { message = "User created successfully", userId = user.Id });
    }

    private string HashPassword(string password)
    {
        // možeš koristiti BCrypt ili SHA256, primer sa BCrypt
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

}