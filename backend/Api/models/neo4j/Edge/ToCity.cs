namespace Api.models.neo4j.Edges;
using Api.models.neo4j.Nodes;
// Route -[TO]-> City
    public class ToCity
    {
        public Route? Route { get; set; }
        public City? City { get; set; }
    }