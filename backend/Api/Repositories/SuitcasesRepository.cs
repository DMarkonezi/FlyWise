using Neo4jClient;
using Api.models.neo4j.Nodes;
using Api.models.neo4j.Edges;
using Neo4jClient.Cypher;

namespace Api.Repositories;

public class SuitcasesRepository
{
    private readonly IGraphClient _client;
    public SuitcasesRepository(IGraphClient client) => _client = client;

    private async Task<int> GetMaxId()
    {
        var result = await _client.Cypher
            .OptionalMatch("(s:Suitcases)")
            .Return(() => Return.As<int>("coalesce(max(s.Id), 0)"))
            .ResultsAsync;
        return result.FirstOrDefault();
    }

    // CREATE: Povezuje Suitcases sa Ticket preko HasSuitcases relacije
    public async Task CreateAsync(Suitcases suitcases, int ticketId, int count)
    {
        suitcases.Id = await GetMaxId() + 1;

        await _client.Cypher
            .Match("(t:Ticket)")
            .Where((Ticket t) => t.Id == ticketId)
            .Create("(s:Suitcases {Id: $id, AllowedWeight: $aWeight})")
            .Create("(t)-[:HAS_SUITCASES {Count: $count}]->(s)")
            .WithParams(new {
                id = suitcases.Id,
                aWeight = suitcases.AllowedWeight,
                count = count
            })
            .ExecuteWithoutResultsAsync();
    }

    // READ ALL (Simple)
    public async Task<List<Suitcases>> GetAllAsync()
    {
        var results = await _client.Cypher
            .Match("(s:Suitcases)")
            .Return(s => s.As<Suitcases>())
            .ResultsAsync;
        return results.ToList();
    }

    // READ BY ID
    public async Task<Suitcases?> GetByIdAsync(int id)
    {
        var results = await _client.Cypher
            .Match("(s:Suitcases)")
            .Where((Suitcases s) => s.Id == id)
            .Return(s => s.As<Suitcases>())
            .ResultsAsync;
        return results.SingleOrDefault();
    }

    // READ DETAILED (Povezivanje putnika, karte i prtljaga sa Count-om)
    public async Task<List<object>> GetDetailedAsync()
    {
        var results = await _client.Cypher
            .Match("(p:Passenger)-[:HAS_TICKET]->(t:Ticket)-[rel:HAS_SUITCASES]->(s:Suitcases)")
            .Return((p, t, rel, s) => new {
                Passenger = p.As<Passenger>().FirstName + " " + p.As<Passenger>().LastName,
                TicketSeat = t.As<Ticket>().SeatNumber,
                SuitcaseId = s.As<Suitcases>().Id,
                AllowedWeight = s.As<Suitcases>().AllowedWeight,
                ItemsCount = rel.As<HasSuitcases>().Count
            })
            .ResultsAsync;
        return results.Cast<object>().ToList();
    }

    // UPDATE
    public async Task UpdateAsync(Suitcases suitcases)
    {
        await _client.Cypher
            .Match("(s:Suitcases)")
            .Where((Suitcases s) => s.Id == suitcases.Id)
            .Set("s.AllowedWeight = $aWeight")
            .WithParam("aWeight", suitcases.AllowedWeight)
            .ExecuteWithoutResultsAsync();
    }

    // DELETE
    public async Task DeleteAsync(int id)
    {
        await _client.Cypher
            .Match("(s:Suitcases)")
            .Where((Suitcases s) => s.Id == id)
            .DetachDelete("s")
            .ExecuteWithoutResultsAsync();
    }
}