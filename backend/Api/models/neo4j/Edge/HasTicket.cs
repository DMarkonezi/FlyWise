namespace Api.models.neo4j.Edges;
using Api.models.neo4j.Nodes;
// Passenger -[HAS_TICKET]-> Ticket
    public class HasTicket
    {
        public Passenger? Passenger { get; set; }
        public Ticket? Ticket { get; set; }
        public DateTime PurchasedAt { get; set; }
    }