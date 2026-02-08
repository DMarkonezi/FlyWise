namespace Api.models.neo4j.Edges;
using Api.models.neo4j.Nodes;
// User -[HAS_TICKET]-> Ticket
    public class HasTicket
    {
        public User? User { get; set; }
        public Ticket? Ticket { get; set; }
        public DateTime PurchasedAt { get; set; }
    }