namespace Api.models.neo4j.Nodes;
using Api.models.neo4j.Edges;
public class Ticket
{
    public int Id { get; set; }
    public string SeatNumber { get; set; } = null!;

    public ForRoute ConnectToRoute(Route route)
    {
        return new ForRoute { Ticket = this, Route = route };
    }

    public HasBill AttachBill(Bill bill)
    {
        return new HasBill { Ticket = this, Bill = bill };
    }
}