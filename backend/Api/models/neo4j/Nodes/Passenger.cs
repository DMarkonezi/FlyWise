namespace Api.models.neo4j.Nodes;
using Api.models.neo4j.Edges;
public class Passenger
{
    public int Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string PassportNumber { get; set; } = null!;
    public string Email { get; set; } = null!;

    public HasTicket PurchaseTicket(Ticket ticket)
    {
        return new HasTicket
        {
            Passenger = this,
            Ticket = ticket,
            PurchasedAt = DateTime.Now
        };
    }
}