namespace Api.models.neo4j.Edges;
using Api.models.neo4j.Nodes;
// Route -[FROM]-> City
    public class FromCity
    {
        public Route? Route { get; set; }
        public City? City { get; set; }
    }
