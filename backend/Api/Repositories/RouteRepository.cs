using Neo4jClient;
using Api.models.neo4j.Nodes;
using Neo4jClient.Cypher;
using Route = Api.models.neo4j.Nodes.Route;

namespace Api.Repositories;

public class RouteRepository
{
    private readonly IGraphClient _client;

    public RouteRepository(IGraphClient client)
    {
        _client = client;
    }

    private async Task<int> GetMaxId()
    {
        var result = await _client.Cypher
            .OptionalMatch("(r:Route)")
            .Return(() => Return.As<int>("coalesce(max(r.Id), 0)"))
            .ResultsAsync;

        return result.FirstOrDefault();
    }

    // CREATE: Pravi rutu i povezuje je sa FROM i TO gradovima
    public async Task CreateAsync(Route route, int fromCityId, int toCityId)
    {
        route.Id = await GetMaxId() + 1;

        await _client.Cypher
            .Match("(f:City)", "(t:City)")
            .Where((City f) => f.Id == fromCityId)
            .AndWhere((City t) => t.Id == toCityId)
            .Create("(r:Route {Id: $id, DepartureTime: $dep, ArrivalTime: $arr})")
            .Create("(r)-[:FROM]->(f)")
            .Create("(r)-[:TO]->(t)")
            .WithParams(new {
                id = route.Id,
                dep = route.DepartureTime.ToString("o"),
                arr = route.ArrivalTime.ToString("o")
            })
            .ExecuteWithoutResultsAsync();
    }

    // READ ALL: Detaljan prikaz rute sa imenima gradova
    public async Task<List<object>> GetAllDetailedAsync()
    {
        var results = await _client.Cypher
            .Match("(f:City)<-[:FROM]-(r:Route)-[:TO]->(t:City)")
            .Return((r, f, t) => new
            {
                RouteId = r.As<Route>().Id,
                Departure = r.As<Route>().DepartureTime,
                Arrival = r.As<Route>().ArrivalTime,
                FromCity = f.As<City>().Name,
                ToCity = t.As<City>().Name
            })
            .ResultsAsync;

        return results.Cast<object>().ToList();
    }

    // READ ONE
    public async Task<Route?> GetByIdAsync(int id)
    {
        var results = await _client.Cypher
            .Match("(r:Route)")
            .Where((Route r) => r.Id == id)
            .Return(r => r.As<Route>())
            .ResultsAsync;

        return results.SingleOrDefault();
    }

    // UPDATE: Menja vremena polaska/dolaska
    public async Task UpdateAsync(Route route)
    {
        await _client.Cypher
            .Match("(r:Route)")
            .Where((Route r) => r.Id == route.Id)
            .Set("r.DepartureTime = $dep, r.ArrivalTime = $arr")
            .WithParams(new {
                dep = route.DepartureTime.ToString("o"),
                arr = route.ArrivalTime.ToString("o")
            })
            .ExecuteWithoutResultsAsync();
    }

    // DELETE
    public async Task DeleteAsync(int id)
    {
        await _client.Cypher
            .Match("(r:Route)")
            .Where((Route r) => r.Id == id)
            .DetachDelete("r")
            .ExecuteWithoutResultsAsync();
    }

    public async Task<object?> GetDetailedByIdAsync(int id)
{
    var results = await _client.Cypher
        .Match("(f:City)<-[:FROM]-(r:Route)-[:TO]->(t:City)")
        .Where((Route r) => r.Id == id)
        .Return((r, f, t) => new
        {
            routeId = r.As<Route>().Id,
            departure = r.As<Route>().DepartureTime,
            arrival = r.As<Route>().ArrivalTime,
            fromCity = f.As<City>().Name,
            toCity = t.As<City>().Name
        })
        .ResultsAsync;

    return results.SingleOrDefault();
}
}