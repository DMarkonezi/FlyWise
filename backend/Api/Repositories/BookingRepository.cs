using Neo4jClient;
using Api.models.neo4j.Nodes;
using Api.models.neo4j.Edges;
using Neo4jClient.Cypher;
using Route = Api.models.neo4j.Nodes.Route;

namespace Api.Repositories;

public class BookingRepository
{
    private readonly IGraphClient _client;
    public BookingRepository(IGraphClient client) => _client = client;

    // Pomoćna funkcija za dobijanje sledećeg ID-a za bilo koju labelu
    private async Task<int> GetNextId(string label)
    {
        var result = await _client.Cypher
            .OptionalMatch($"(n:{label})")
            .Return(() => Return.As<int>("coalesce(max(n.Id), 0) + 1"))
            .ResultsAsync;
        return result.First();
    }

    // 1. REZERVACIJA (Glavna funkcija)
    public async Task CreateBookingAsync(int passengerId, int routeId, string seatNumber, double suitcaseWeight, int suitcaseCount, double seatPrice, double suitcasePrice)
    {
        int ticketId = await GetNextId("Ticket");
        int billId = await GetNextId("Bill");
        int suitcaseId = await GetNextId("Suitcases");

        await _client.Cypher
            .Match("(p:Passenger)", "(r:Route)")
            .Where((Passenger p) => p.Id == passengerId)
            .AndWhere((Route r) => r.Id == routeId)
            // Kreiranje Ticket-a i veza
            .Create("(t:Ticket {Id: $tId, SeatNumber: $seat})")
            .Create("(p)-[:HAS_TICKET {PurchasedAt: $now}]->(t)")
            .Create("(t)-[:FOR_ROUTE]->(r)")
            // Kreiranje Bill-a i veze
            .Create("(b:Bill {Id: $bId, TotalAmount: $total, SeatPrice: $sPrice, SuitcasesPrice: $suitPrice, IssuedAt: $now})")
            .Create("(t)-[:HAS_BILL]->(b)")
            // Kreiranje Suitcases i veze
            .Create("(s:Suitcases {Id: $sId, AllowedWeight: $weight})")
            .Create("(t)-[:HAS_SUITCASES {Count: $count}]->(s)")
            .WithParams(new {
                tId = ticketId, seat = seatNumber,
                bId = billId, total = seatPrice + suitcasePrice, sPrice = seatPrice, suitPrice = suitcasePrice,
                sId = suitcaseId, weight = suitcaseWeight, count = suitcaseCount,
                now = DateTime.Now.ToString("o")
            })
            .ExecuteWithoutResultsAsync();
    }

// 2. IZMENA REZERVACIJE - Pokriva sve atribute karte, prtljaga i računa
public async Task UpdateBookingAsync(int ticketId, string newSeat, double newWeight, int newCount, double newSeatPrice, double newSuitcasePrice)
{
    await _client.Cypher
        .Match("(t:Ticket)-[relS:HAS_SUITCASES]->(s:Suitcases)")
        .Match("(t)-[:HAS_BILL]->(b:Bill)")
        .Where((Ticket t) => t.Id == ticketId)
        .Set("t.SeatNumber = $seat, " +
             "s.AllowedWeight = $weight, " +
             "relS.Count = $count, " +
             "b.SeatPrice = $sPrice, " +
             "b.SuitcasesPrice = $suitPrice, " +
             "b.TotalAmount = $total")
        .WithParams(new { 
            seat = newSeat, 
            weight = newWeight, 
            count = newCount,
            sPrice = newSeatPrice,
            suitPrice = newSuitcasePrice,
            total = newSeatPrice + newSuitcasePrice // Automatski kalkulišemo novi total
        })
        .ExecuteWithoutResultsAsync();
}

    // 3. PONIŠTAVANJE (Briše kartu i sve što je vezano samo za nju: Bill, Suitcases)
    public async Task CancelBookingAsync(int ticketId)
    {
        await _client.Cypher
            .Match("(t:Ticket)")
            .Where((Ticket t) => t.Id == ticketId)
            .OptionalMatch("(t)-[:HAS_BILL]->(b:Bill)")
            .OptionalMatch("(t)-[:HAS_SUITCASES]->(s:Suitcases)")
            .DetachDelete("t, b, s")
            .ExecuteWithoutResultsAsync();
    }
}