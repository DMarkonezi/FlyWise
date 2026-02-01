using Neo4jClient;
using Api.models.neo4j.Nodes;
using Neo4jClient.Cypher;
using Route = Api.models.neo4j.Nodes.Route;

namespace Api.Repositories;

public class FilterRepository
{
    private readonly IGraphClient _client;
    public FilterRepository(IGraphClient client) => _client = client;

    // 1. Filtriranje po ID-evima gradova (from: 1, to: 2)
    public async Task<List<object>> GetRoutesBetweenCitiesAsync(int fromCityId, int toCityId)
    {
        var results = await _client.Cypher
            .Match("(cFrom:City)<-[:FROM]-(r:Route)-[:TO]->(cTo:City)")
            .Where((City cFrom) => cFrom.Id == fromCityId)
            .AndWhere((City cTo) => cTo.Id == toCityId)
            .Return((r, cFrom, cTo) => new
            {
                RouteId = r.As<Route>().Id,
                Departure = r.As<Route>().DepartureTime,
                Arrival = r.As<Route>().ArrivalTime,
                From = cFrom.As<City>().Name,
                To = cTo.As<City>().Name
            })
            .ResultsAsync;

        return results.Cast<object>().ToList();
    }

    // 2. Filtriranje po ID-evima država
    public async Task<List<object>> GetRoutesBetweenCountriesAsync(int fromCountryId, int toCountryId)
    {
        var results = await _client.Cypher
            .Match("(coFrom:Country)<-[:IN_COUNTRY]-(cFrom:City)<-[:FROM]-(r:Route)-[:TO]->(cTo:City)-[:IN_COUNTRY]->(coTo:Country)")
            .Where((Country coFrom) => coFrom.Id == fromCountryId)
            .AndWhere((Country coTo) => coTo.Id == toCountryId)
            .Return((r, cFrom, cTo, coFrom, coTo) => new
            {
                RouteId = r.As<Route>().Id,
                Departure = r.As<Route>().DepartureTime,
                Arrival = r.As<Route>().ArrivalTime,
                OriginCity = cFrom.As<City>().Name,
                DestinationCity = cTo.As<City>().Name
            })
            .ResultsAsync;

        return results.Cast<object>().ToList();
    }
}