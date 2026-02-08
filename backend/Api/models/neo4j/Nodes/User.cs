namespace Api.models.neo4j.Nodes;
using Api.models.neo4j.Edges;

public class User
{
    public int Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;

    public string PassportNumber {get; set;} = null!;
    public DateTime CreatedAt { get; set; }

    // Opcional - za relaciju sa tickets
    public HasTicket PurchaseTicket(Ticket ticket)
    {
        return new HasTicket
        {
            User = this,
            Ticket = ticket,
            PurchasedAt = DateTime.Now
        };
    }

    public string Name => $"{FirstName} {LastName}";
}