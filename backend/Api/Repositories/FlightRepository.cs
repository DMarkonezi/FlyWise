using Neo4jClient;
using Api.models.neo4j.Nodes;
using Api.models.neo4j.Edges;
using Neo4jClient.Cypher;
using Route = Api.models.neo4j.Nodes.Route;

namespace Api.Repositories;

public class FlightRepository
{
    private readonly IGraphClient _client;

    public FlightRepository(IGraphClient client)
    {
        _client = client;
    }

    private async Task<int> GetMaxId()
    {
        var result = await _client.Cypher
            .OptionalMatch("(f:Flight)")
            .Return(() => Return.As<int>("coalesce(max(f.Id), 0)"))
            .ResultsAsync;

        return result.FirstOrDefault();
    }

    // 1. CREATE
    public async Task CreateAsync(Flight flight, int routeId, int order)
    {
        flight.Id = await GetMaxId() + 1;

        await _client.Cypher
            .Match("(r:Route)")
            .Where((Route r) => r.Id == routeId)
            .Create("(f:Flight {Id: $id, FlightNumber: $fNum})")
            .Create("(f)-[:HAS_ROUTE {Order: $ord}]->(r)")
            .WithParams(new {
                id = flight.Id,
                fNum = flight.FlightNumber,
                ord = order
            })
            .ExecuteWithoutResultsAsync();
    }

    // 2. READ ALL (Simple)
    public async Task<List<Flight>> GetAllAsync()
    {
        var results = await _client.Cypher
            .Match("(f:Flight)")
            .Return(f => f.As<Flight>())
            .ResultsAsync;

        return results.ToList();
    }

    // 3. READ ALL (Detailed - sa gradovima i vremenima)
    public async Task<List<object>> GetAllDetailedAsync()
    {
        var results = await _client.Cypher
            .Match("(f:Flight)-[rel:HAS_ROUTE]->(r:Route)")
            .Match("(from:City)<-[:FROM_CITY]-(r)-[:TO_CITY]->(to:City)")
            .Return((f, rel, r, from, to) => new
            {
                FlightId = f.As<Flight>().Id,
                Number = f.As<Flight>().FlightNumber,
                Order = rel.As<HasRoute>().Order,
                Departure = r.As<Route>().DepartureTime,
                Arrival = r.As<Route>().ArrivalTime,
                From = from.As<City>().Name,
                To = to.As<City>().Name
            })
            .ResultsAsync;

        return results.Cast<object>().ToList();
    }

    // 4. READ ONE
    public async Task<Flight?> GetByIdAsync(int id)
    {
        var results = await _client.Cypher
            .Match("(f:Flight)")
            .Where((Flight f) => f.Id == id)
            .Return(f => f.As<Flight>())
            .ResultsAsync;

        return results.SingleOrDefault();
    }

    // 5. UPDATE (Samo osnovni podaci leta)
    public async Task UpdateAsync(Flight flight)
    {
        await _client.Cypher
            .Match("(f:Flight)")
            .Where((Flight f) => f.Id == flight.Id)
            .Set("f.FlightNumber = $fNum")
            .WithParam("fNum", flight.FlightNumber)
            .ExecuteWithoutResultsAsync();
    }

    // 6. DELETE
    public async Task DeleteAsync(int id)
    {
        await _client.Cypher
            .Match("(f:Flight)")
            .Where((Flight f) => f.Id == id)
            .DetachDelete("f")
            .ExecuteWithoutResultsAsync();
    }
}