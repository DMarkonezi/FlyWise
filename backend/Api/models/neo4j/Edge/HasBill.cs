namespace Api.models.neo4j.Edges;
using Api.models.neo4j.Nodes;
// Ticket -[HAS_BILL]-> Bill
    public class HasBill
    {
        public Ticket? Ticket { get; set; }
        public Bill? Bill { get; set; }
    }