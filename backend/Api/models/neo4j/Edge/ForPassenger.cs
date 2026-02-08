namespace Api.Models.neo4j.Edges;

using Api.models.neo4j.Nodes;

// Ticket -[FOR_PASSENGER]-> Passenger
public class ForPassenger
{
    public Ticket? Ticket { get; set; }
    public Passenger? Passenger { get; set; }
}