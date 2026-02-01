using Neo4jClient;
using Api.models.neo4j.Nodes;
using Neo4jClient.Cypher;

namespace Api.Repositories;

public class CityRepository
{
    private readonly IGraphClient _client;

    public CityRepository(IGraphClient client)
    {
        _client = client;
    }

    // Pomoćna metoda za automatski ID
 private async Task<int> GetMaxId()
{
    var result = await _client.Cypher
        .OptionalMatch("(c:City)") // OptionalMatch je sigurniji kad je baza prazna
        .Return(() => Return.As<int>("coalesce(max(c.Id), 0)"))
        .ResultsAsync;

    return result.FirstOrDefault();
}

    // CREATE: Pravi grad i povezuje ga sa postojećom državom
    public async Task CreateAsync(City city, int countryId)
    {
        int nextId = await GetMaxId() + 1;
        city.Id = nextId;

        await _client.Cypher
            .Match("(co:Country)")
            .Where((Country co) => co.Id == countryId)
            .Create("(ci:City {Id: $id, Name: $name})")
            .Create("(ci)-[:IN_COUNTRY]->(co)")
            .WithParams(new {
                id = city.Id,
                name = city.Name
            })
            .ExecuteWithoutResultsAsync();
    }

    // READ: Dohvata gradove zajedno sa imenom države (korisno za prikaz)
    public async Task<List<object>> GetAllAsync()
    {
        var results = await _client.Cypher
            .Match("(ci:City)-[:IN_COUNTRY]->(co:Country)")
            .Return((ci, co) => new
            {
                Id = ci.As<City>().Id,
                CityName = ci.As<City>().Name,
                CountryName = co.As<Country>().Name
            })
            .ResultsAsync;

        return results.Cast<object>().ToList();
    }

    // READ: Dohvata jedan grad po ID-u
    public async Task<object?> GetByIdAsync(int id)
    {
        var results = await _client.Cypher
            .Match("(ci:City)-[:IN_COUNTRY]->(co:Country)")
            .Where((City ci) => ci.Id == id)
            .Return((ci, co) => new
            {
                Id = ci.As<City>().Id,
                CityName = ci.As<City>().Name,
                CountryName = co.As<Country>().Name
            })
            .ResultsAsync;

        return results.SingleOrDefault();
    }

    // UPDATE: Menja ime grada
    public async Task UpdateAsync(City city)
    {
        await _client.Cypher
            .Match("(ci:City)")
            .Where((City ci) => ci.Id == city.Id)
            .Set("ci.Name = $name")
            .WithParam("name", city.Name)
            .ExecuteWithoutResultsAsync();
    }

    // DELETE: Brisanje grada i njegovih relacija
    public async Task DeleteAsync(int id)
    {
        await _client.Cypher
            .Match("(ci:City)")
            .Where((City ci) => ci.Id == id)
            .DetachDelete("ci")
            .ExecuteWithoutResultsAsync();
    }
}