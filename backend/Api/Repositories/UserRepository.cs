using Neo4jClient;
using Api.models.neo4j.Nodes;
using Api.models.neo4j.Edges;
using Neo4jClient.Cypher;

namespace Api.Repositories;

public class UserRepository
{
    private readonly IGraphClient _client;

    public UserRepository(IGraphClient client)
    {
        _client = client;
    }

    private async Task<int> GetMaxId()
    {
        var result = await _client.Cypher
            .OptionalMatch("(p:User)")
            .Return(() => Return.As<int>("coalesce(max(p.Id), 0)"))
            .ResultsAsync;

        return result.FirstOrDefault();
    }

    // CREATE: Pravi novog putnika
    public async Task CreateAsync(User user)
    {
        user.Id = await GetMaxId() + 1;

        await _client.Cypher
            .Create("(p:User {Id: $id, FirstName: $fn, LastName: $ln, PassportNumber: $pn, Email: $email, PasswordHash: $ph})")
            .WithParams(new {
                id = user.Id,
                fn = user.FirstName,
                ln = user.LastName,
                pn = user.PassportNumber,
                email = user.Email,
                ph = user.PasswordHash
            })
            .ExecuteWithoutResultsAsync();
    }

    // READ ALL
    public async Task<List<User>> GetAllAsync()
    {
        var results = await _client.Cypher
            .Match("(p:User)")
            .Return(p => p.As<User>())
            .ResultsAsync;

        return results.ToList();
    }

    // READ ONE (Detailed - sa kartama koje poseduje)
    public async Task<object?> GetDetailedAsync(int id)
    {
        var results = await _client.Cypher
            .Match("(p:User)")
            .Where((User p) => p.Id == id)
            .OptionalMatch("(p)-[rel:HAS_TICKET]->(t:Ticket)")
            .Return((p, rel, t) => new
            {
                User = p.As<User>(),
                Tickets = t.CollectAs<Ticket>(), // Skuplja sve karte u listu
                PurchaseDates = rel.CollectAs<HasTicket>() // Skuplja podatke sa relacija
            })
            .ResultsAsync;

        return results.SingleOrDefault();
    }

    // UPDATE
    public async Task UpdateAsync(User user)
    {
        await _client.Cypher
            .Match("(p:User)")
            .Where((User p) => p.Id == user.Id)
            .Set("p.FirstName = $fn, p.LastName = $ln, p.PassportNumber = $pn, p.Email = $email")
            .WithParams(new {
                fn = user.FirstName,
                ln = user.LastName,
                pn = user.PassportNumber,
                email = user.Email
            })
            .ExecuteWithoutResultsAsync();
    }

    // DELETE
    public async Task DeleteAsync(int id)
    {
        await _client.Cypher
            .Match("(p:User)")
            .Where((User p) => p.Id == id)
            .DetachDelete("p")
            .ExecuteWithoutResultsAsync();
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        var results = await _client.Cypher
            .Match("(u:User)")
            .Where((User u) => u.Email == email)
            .Return(u => u.As<User>())
            .ResultsAsync;

        return results.SingleOrDefault();
    }
}