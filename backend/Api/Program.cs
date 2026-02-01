using Neo4j.Driver;           // Za Driver
using Neo4jClient;           // Za IGraphClient (Fakultetski stil)
using Api.Data;               // Za Neo4jInitializer
using Api.Repositories;       // Za tvoje repozitorijume

var builder = WebApplication.CreateBuilder(args);

// --- 1. KONFIGURACIJA IZ appsettings.json ---
var neo4jConfig = builder.Configuration.GetSection("Neo4j");
var uri = neo4jConfig["Uri"] ?? "bolt://localhost:7687";
var user = neo4jConfig["User"] ?? "neo4j";
var password = neo4jConfig["Password"] ?? "password";

// --- 2. NEO4J DRIVER (Potreban za Initializer) ---
builder.Services.AddSingleton(GraphDatabase.Driver(
    uri, 
    AuthTokens.Basic(user, password)
));

// --- 3. NEO4J CLIENT (Fakultetski stil - IGraphClient) ---
var client = new GraphClient(new Uri("http://localhost:7474"), user, password); // Neo4jClient koristi HTTP port 7474
try 
{
    await client.ConnectAsync();
    builder.Services.AddSingleton<IGraphClient>(client);
}
catch (Exception ex)
{
    Console.WriteLine($"Greška pri povezivanju na Neo4jClient: {ex.Message}");
}

// --- 4. REGISTRACIJA REPOZITORIJUMA ---
builder.Services.AddScoped<CountryRepository>();
builder.Services.AddScoped<CityRepository>();
builder.Services.AddScoped<BillRepository>();
builder.Services.AddScoped<FlightRepository>();
builder.Services.AddScoped<RouteRepository>();
builder.Services.AddScoped<PassengerRepository>();
builder.Services.AddScoped<TicketRepository>();
builder.Services.AddScoped<SuitcasesRepository>();
// --- 5. INICIJALIZACIJA BAZE ---
builder.Services.AddSingleton<Neo4jInitializer>();

// --- 6. STANDARDNE SERVISNE KONFIGURACIJE ---
builder.Services.AddControllers(); 
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// --- 7. POKRETANJE INICIJALIZACIJE (Constraints/Indexes) ---
using (var scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider.GetRequiredService<Neo4jInitializer>();
    await initializer.InitializeAsync();
}

// --- 8. MIDDLEWARE (HTTP Pipeline) ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

// Mapiranje ruta za kontrolere (bitno za [Route("api/[controller]")])
app.MapControllers(); 

// Pomoćna ruta za proveru da li API radi
app.MapGet("/", () => "FlyWise API is running...");

app.Run();