using Neo4j.Driver;

namespace Api.Data;

public class Neo4jInitializer
{
    private readonly IDriver _driver;

    public Neo4jInitializer(IConfiguration config)
    {
        var section = config.GetSection("Neo4j");
        _driver = GraphDatabase.Driver(section["Uri"], AuthTokens.Basic(section["User"], section["Password"]));
    }

    public async Task InitializeAsync()
    {
        await using var session = _driver.AsyncSession();
        
        await session.ExecuteWriteAsync(async tx => {
            // Unikatni ključevi (zamena za Primary Key u SQL-u)
            await tx.RunAsync("CREATE CONSTRAINT IF NOT EXISTS FOR (p:Passenger) REQUIRE p.PassportNumber IS UNIQUE");
            await tx.RunAsync("CREATE CONSTRAINT IF NOT EXISTS FOR (f:Flight) REQUIRE f.FlightNumber IS UNIQUE");
            await tx.RunAsync("CREATE CONSTRAINT IF NOT EXISTS FOR (c:City) REQUIRE c.Id IS UNIQUE");
        });
        
        Console.WriteLine("Neo4j šema je uspešno inicijalizovana!");
    }
}