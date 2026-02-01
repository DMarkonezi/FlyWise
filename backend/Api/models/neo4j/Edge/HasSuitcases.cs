namespace Api.models.neo4j.Edges;
using Api.models.neo4j.Nodes;
// Ticket -[HAS_SUITCASES]-> Suitcases
    public class HasSuitcases
    {
        public Ticket? Ticket { get; set; }
        public Suitcases? Suitcases { get; set; }
        public int Count { get; set; }
    }