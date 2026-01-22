public class Flight
{
    public string FlightId { get; set; } 
    public string FlightNumber { get; set; }
    public string AircraftType { get; set; }
    public string AirlineId { get; set; }
    public string Status { get; set; }

    public DateTime DepartureTime { get; set; }
    public DateTime ArrivalTime { get; set; }

    public int NumberOfSeats { get; set; }
    public int AvailableSeats { get; set; }

    public decimal Price {get; set;}

    public AirPort DepartureDestination { get; set; }
    public AirPort ArrivalDestination { get; set; }
}