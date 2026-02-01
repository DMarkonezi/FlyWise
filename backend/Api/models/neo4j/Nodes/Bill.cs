namespace Api.models.neo4j.Nodes;
public class Bill
{
    public int Id { get; set; }
    public double TotalAmount { get; set; }
    public double SeatPrice { get; set; }
    public double SuitcasesPrice { get; set; }
    public DateTime IssuedAt { get; set; }
}