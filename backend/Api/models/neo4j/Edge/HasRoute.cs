namespace Api.models.neo4j.Edges;
using Api.models.neo4j.Nodes;
// Flight -[HAS_ROUTE]-> Route
    public class HasRoute
    {
        public Flight? Flight { get; set; }
        public Route? Route { get; set; }
        public int Order { get; set; }
    }