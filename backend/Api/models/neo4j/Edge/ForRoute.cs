namespace Api.models.neo4j.Edges;
using Api.models.neo4j.Nodes;
// Ticket -[FOR_ROUTE]-> Route
    public class ForRoute
    {
        public Ticket? Ticket { get; set; }
        public Route? Route { get; set; }
    }