using Neo4jClient;
using Api.models.neo4j.Nodes;
using Neo4jClient.Cypher;

namespace Api.Repositories;

public class PassengerRepository
{
    private readonly IGraphClient _client;

    public PassengerRepository(IGraphClient client)
    {
        _client = client;
    }

    private async Task<int> GetMaxId()
    {
        var result = await _client.Cypher
            .OptionalMatch("(p:Passenger)")
            .Return(() => Return.As<int>("coalesce(max(p.Id), 0)"))
            .ResultsAsync;

        return result.FirstOrDefault();
    }

    // CREATE Passenger + poveži ga sa User-om
    public async Task CreateAsync(Passenger passenger, int userId)
    {
        passenger.Id = await GetMaxId() + 1;

        await _client.Cypher
            .Match("(u:User)")
            .Where((User u) => u.Id == userId)
            .Create(@"(p:Passenger {
                        Id: $id,
                        FirstName: $fn,
                        LastName: $ln,
                        PassportNumber: $pn
                     })")
            .Create("(u)-[:OWNS]->(p)")
            .WithParams(new
            {
                id = passenger.Id,
                fn = passenger.FirstName,
                ln = passenger.LastName,
                pn = passenger.PassportNumber
            })
            .ExecuteWithoutResultsAsync();
    }

    // READ ALL
    public async Task<List<Passenger>> GetAllAsync()
    {
        var results = await _client.Cypher
            .Match("(p:Passenger)")
            .Return(p => p.As<Passenger>())
            .ResultsAsync;

        return results.ToList();
    }

    // READ Passenger by ID
    public async Task<Passenger?> GetByIdAsync(int id)
    {
        var results = await _client.Cypher
            .Match("(p:Passenger)")
            .Where((Passenger p) => p.Id == id)
            .Return(p => p.As<Passenger>())
            .ResultsAsync;

        return results.SingleOrDefault();
    }

    // READ all passengers for one user
    public async Task<List<Passenger>> GetByUserAsync(int userId)
    {
        var results = await _client.Cypher
            .Match("(u:User)-[:OWNS]->(p:Passenger)")
            .Where((User u) => u.Id == userId)
            .Return(p => p.As<Passenger>())
            .ResultsAsync;

        return results.ToList();
    }

    // UPDATE
    public async Task UpdateAsync(Passenger passenger)
    {
        await _client.Cypher
            .Match("(p:Passenger)")
            .Where((Passenger p) => p.Id == passenger.Id)
            .Set(@"p.FirstName = $fn,
                   p.LastName = $ln,
                   p.PassportNumber = $pn")
            .WithParams(new
            {
                fn = passenger.FirstName,
                ln = passenger.LastName,
                pn = passenger.PassportNumber
            })
            .ExecuteWithoutResultsAsync();
    }

    // DELETE
    public async Task DeleteAsync(int id)
    {
        await _client.Cypher
            .Match("(p:Passenger)")
            .Where((Passenger p) => p.Id == id)
            .DetachDelete("p")
            .ExecuteWithoutResultsAsync();
    }
}
