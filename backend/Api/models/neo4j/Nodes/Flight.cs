namespace Api.models.neo4j.Nodes;
using Api.models.neo4j.Edges;
public class Flight
{
    public int Id { get; set; }
    public string FlightNumber { get; set; } = null!;
    
    public HasRoute AssignRoute(Route route, int order)
    {
        return new HasRoute
        {
            Flight = this,
            Route = route,
            Order = order
        };
    }
}