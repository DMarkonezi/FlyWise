using Neo4jClient;
using Api.models.neo4j.Nodes;
using Api.models.neo4j.Edges;
using Neo4jClient.Cypher;
using Route = Api.models.neo4j.Nodes.Route;

namespace Api.Repositories;

public class TicketRepository
{
    private readonly IGraphClient _client;
    public TicketRepository(IGraphClient client) => _client = client;

    private async Task<int> GetMaxId()
    {
        var result = await _client.Cypher
            .OptionalMatch("(t:Ticket)")
            .Return(() => Return.As<int>("coalesce(max(t.Id), 0)"))
            .ResultsAsync;
        return result.FirstOrDefault();
    }

    // 1. CREATE
    public async Task CreateAsync(Ticket ticket, int passengerId, int routeId)
    {
        ticket.Id = await GetMaxId() + 1;
        await _client.Cypher
            .Match("(p:Passenger)", "(r:Route)")
            .Where((Passenger p) => p.Id == passengerId)
            .AndWhere((Route r) => r.Id == routeId)
            .Create("(t:Ticket {Id: $id, SeatNumber: $seat})")
            .Create("(p)-[:HAS_TICKET {PurchasedAt: $now}]->(t)")
            .Create("(t)-[:FOR_ROUTE]->(r)")
            .WithParams(new {
                id = ticket.Id,
                seat = ticket.SeatNumber,
                now = DateTime.Now.ToString("o")
            })
            .ExecuteWithoutResultsAsync();
    }

    // 2. READ (Simple GetAll)
    public async Task<List<Ticket>> GetAllAsync()
    {
        var results = await _client.Cypher
            .Match("(t:Ticket)")
            .Return(t => t.As<Ticket>())
            .ResultsAsync;
        return results.ToList();
    }

    // 3. READ (GetById)
    public async Task<Ticket?> GetByIdAsync(int id)
    {
        var results = await _client.Cypher
            .Match("(t:Ticket)")
            .Where((Ticket t) => t.Id == id)
            .Return(t => t.As<Ticket>())
            .ResultsAsync;
        return results.SingleOrDefault();
    }

    // 4. READ (Detailed)
    public async Task<List<object>> GetAllDetailedAsync()
    {
        var results = await _client.Cypher
            .Match("(p:Passenger)-[:HAS_TICKET]->(t:Ticket)-[:FOR_ROUTE]->(r:Route)")
            .Match("(r)-[:FROM]->(from:City), (r)-[:TO]->(to:City)")
            .Return((p, t, r, from, to) => new
            {
                TicketId = t.As<Ticket>().Id,
                Seat = t.As<Ticket>().SeatNumber,
                PassengerName = p.As<Passenger>().FirstName + " " + p.As<Passenger>().LastName,
                DepartureTime = r.As<Route>().DepartureTime,
                RoutePath = from.As<City>().Name + " -> " + to.As<City>().Name
            })
            .ResultsAsync;
        return results.Cast<object>().ToList();
    }

    // 5. UPDATE
    public async Task UpdateAsync(Ticket ticket)
    {
        await _client.Cypher
            .Match("(t:Ticket)")
            .Where((Ticket t) => t.Id == ticket.Id)
            .Set("t.SeatNumber = $seat")
            .WithParam("seat", ticket.SeatNumber)
            .ExecuteWithoutResultsAsync();
    }

    // 6. DELETE
    public async Task DeleteAsync(int id)
    {
        await _client.Cypher
            .Match("(t:Ticket)")
            .Where((Ticket t) => t.Id == id)
            .DetachDelete("t")
            .ExecuteWithoutResultsAsync();
    }
}