namespace Api.models.neo4j.Edges;
using Api.models.neo4j.Nodes;
// City -[IN_COUNTRY]-> Country
    public class InCountry
    {
        public City? City { get; set; }
        public Country? Country { get; set; }
    }