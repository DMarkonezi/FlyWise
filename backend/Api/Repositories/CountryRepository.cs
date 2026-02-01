using Neo4jClient;
using Api.models.neo4j.Nodes;
using Neo4jClient.Cypher;

namespace Api.Repositories;

public class CountryRepository
{
    private readonly IGraphClient _client;

    public CountryRepository(IGraphClient client)
    {
        _client = client;
    }

    // Pomoćna metoda za Max ID (kao što ste radili na faksu)
private async Task<int> GetMaxId()
{
    var result = await _client.Cypher
        .OptionalMatch("(c:Country)") // Koristimo OptionalMatch da ne pukne ako nema ničega
        .Return(() => Return.As<int>("coalesce(max(c.Id), 0)")) // Pretvara null u 0
        .ResultsAsync;

    return result.FirstOrDefault();
}

    // CREATE
    public async Task CreateAsync(Country country)
    {
        int maxId = await GetMaxId();
        country.Id = maxId + 1;

        await _client.Cypher
            .Create("(c:Country {Id: $id, Name: $name})")
            .WithParams(new
            {
                id = country.Id,
                name = country.Name
            })
            .ExecuteWithoutResultsAsync();
    }

    // READ ALL
    public async Task<List<Country>> GetAllAsync()
    {
        var countries = await _client.Cypher
            .Match("(c:Country)")
            .Return(c => c.As<Country>())
            .ResultsAsync;

        return countries.ToList();
    }

    // READ ONE
    public async Task<Country?> GetByIdAsync(int id)
    {
        var countries = await _client.Cypher
            .Match("(c:Country)")
            .Where((Country c) => c.Id == id)
            .Return(c => c.As<Country>())
            .ResultsAsync;

        return countries.SingleOrDefault();
    }

    // UPDATE
    public async Task UpdateAsync(Country country)
    {
        await _client.Cypher
            .Match("(c:Country)")
            .Where((Country c) => c.Id == country.Id)
            .Set("c.Name = $name")
            .WithParam("name", country.Name)
            .ExecuteWithoutResultsAsync();
    }

    // DELETE
    public async Task DeleteAsync(int id)
    {
        await _client.Cypher
            .Match("(c:Country)")
            .Where((Country c) => c.Id == id)
            .DetachDelete("c")
            .ExecuteWithoutResultsAsync();
    }
}