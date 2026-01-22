public class Ticket
{
    public string TickeId { get; set; }
    public string UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public decimal TotalPrice {get; set;}
    public List<Flight> Flights { get; set; }
}