using Microsoft.AspNetCore.Mvc;
using Api.Repositories;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FilterController : ControllerBase
{
    private readonly FilterRepository _repo;
    public FilterController(FilterRepository repo) => _repo = repo;

    // Sada unosiš ID-eve: api/Filter/cities?from=1&to=2
    [HttpGet("cities")]
    public async Task<IActionResult> GetByCities([FromQuery] int from, [FromQuery] int to)
    {
        var results = await _repo.GetRoutesBetweenCitiesAsync(from, to);
        return Ok(results);
    }

    // api/Filter/countries?from=1&to=2
    [HttpGet("countries")]
    public async Task<IActionResult> GetByCountries([FromQuery] int from, [FromQuery] int to)
    {
        var results = await _repo.GetRoutesBetweenCountriesAsync(from, to);
        return Ok(results);
    }
}