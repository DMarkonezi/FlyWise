using Neo4jClient;
using Api.models.neo4j.Nodes;
using Api.models.neo4j.Edges;
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

    // CREATE: Pravi novog putnika
    public async Task CreateAsync(Passenger passenger)
    {
        passenger.Id = await GetMaxId() + 1;

        await _client.Cypher
            .Create("(p:Passenger {Id: $id, FirstName: $fn, LastName: $ln, PassportNumber: $pn, Email: $email})")
            .WithParams(new {
                id = passenger.Id,
                fn = passenger.FirstName,
                ln = passenger.LastName,
                pn = passenger.PassportNumber,
                email = passenger.Email
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

    // READ ONE (Detailed - sa kartama koje poseduje)
    public async Task<object?> GetDetailedAsync(int id)
    {
        var results = await _client.Cypher
            .Match("(p:Passenger)")
            .Where((Passenger p) => p.Id == id)
            .OptionalMatch("(p)-[rel:HAS_TICKET]->(t:Ticket)")
            .Return((p, rel, t) => new
            {
                Passenger = p.As<Passenger>(),
                Tickets = t.CollectAs<Ticket>(), // Skuplja sve karte u listu
                PurchaseDates = rel.CollectAs<HasTicket>() // Skuplja podatke sa relacija
            })
            .ResultsAsync;

        return results.SingleOrDefault();
    }

    // UPDATE
    public async Task UpdateAsync(Passenger passenger)
    {
        await _client.Cypher
            .Match("(p:Passenger)")
            .Where((Passenger p) => p.Id == passenger.Id)
            .Set("p.FirstName = $fn, p.LastName = $ln, p.PassportNumber = $pn, p.Email = $email")
            .WithParams(new {
                fn = passenger.FirstName,
                ln = passenger.LastName,
                pn = passenger.PassportNumber,
                email = passenger.Email
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