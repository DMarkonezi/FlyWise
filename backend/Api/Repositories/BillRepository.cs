using Neo4jClient;
using Api.models.neo4j.Nodes;
using Neo4jClient.Cypher;

namespace Api.Repositories;

public class BillRepository
{
    private readonly IGraphClient _client;

    public BillRepository(IGraphClient client)
    {
        _client = client;
    }

    // Pomoćna metoda za siguran ID
    private async Task<int> GetMaxId()
    {
        var result = await _client.Cypher
            .OptionalMatch("(b:Bill)")
            .Return(() => Return.As<int>("coalesce(max(b.Id), 0)"))
            .ResultsAsync;

        return result.FirstOrDefault();
    }

    // CREATE: Pravi račun i povezuje ga sa kartom (Ticket)
    public async Task CreateBillAsync(Bill bill, int ticketId)
    {
        int nextId = await GetMaxId() + 1;
        bill.Id = nextId;
        
        // Postavljamo trenutno vreme ako nije definisano
        if (bill.IssuedAt == default) bill.IssuedAt = DateTime.Now;

        await _client.Cypher
            .Match("(t:Ticket)")
            .Where((Ticket t) => t.Id == ticketId)
            .Create("(b:Bill {Id: $id, TotalAmount: $total, SeatPrice: $sPrice, SuitcasesPrice: $suitPrice, IssuedAt: $issued})")
            .Create("(t)-[:HAS_BILL]->(b)")
            .WithParams(new {
                id = bill.Id,
                total = bill.TotalAmount,
                sPrice = bill.SeatPrice,
                suitPrice = bill.SuitcasesPrice,
                issued = bill.IssuedAt.ToString("o") // ISO 8601 format
            })
            .ExecuteWithoutResultsAsync();
    }

    // READ: Dohvata jedan račun
    public async Task<Bill?> GetBillByIdAsync(int id)
    {
        var results = await _client.Cypher
            .Match("(b:Bill)")
            .Where((Bill b) => b.Id == id)
            .Return(b => b.As<Bill>())
            .ResultsAsync;

        return results.SingleOrDefault();
    }

    // UPDATE: Menja podatke na računu
    public async Task UpdateBillAsync(Bill bill)
    {
        await _client.Cypher
            .Match("(b:Bill)")
            .Where((Bill b) => b.Id == bill.Id)
            .Set("b.TotalAmount = $total, b.SeatPrice = $sPrice, b.SuitcasesPrice = $suitPrice")
            .WithParams(new {
                total = bill.TotalAmount,
                sPrice = bill.SeatPrice,
                suitPrice = bill.SuitcasesPrice
            })
            .ExecuteWithoutResultsAsync();
    }

    // DELETE: Briše račun i sve veze
    public async Task DeleteBillAsync(int id)
    {
        await _client.Cypher
            .Match("(b:Bill)")
            .Where((Bill b) => b.Id == id)
            .DetachDelete("b")
            .ExecuteWithoutResultsAsync();
    }

    public async Task<Bill?> GetBillByTicketIdAsync(int ticketId)
    {
        var result = await _client.Cypher
            .Match("(b:Bill)-[:BELONGS_TO]->(t:Ticket {TicketId: $ticketId})")
            .WithParam("ticketId", ticketId)
            .Return(b => b.As<Bill>())
            .ResultsAsync;

        return result.FirstOrDefault();
    }
}