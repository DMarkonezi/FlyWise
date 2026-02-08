// using Neo4j.Driver;           // Za Driver
// using Neo4jClient;           // Za IGraphClient 
// using Api.Data;               // Za Neo4jInitializer
// using Api.Repositories;       //
// using StackExchange.Redis;    
// using Utilities;              
// using RedisDataLayer;         
// using Data.Redis;

// var builder = WebApplication.CreateBuilder(args);

// builder.Services.AddCors(options =>
// {
//     options.AddDefaultPolicy(policy =>
//     {
//         policy.WithOrigins("http://localhost:5173") // frontend URL
//               .AllowAnyHeader()
//               .AllowAnyMethod();
//     });
// });

// // --- 1. KONFIGURACIJA IZ appsettings.json ---
// var neo4jConfig = builder.Configuration.GetSection("Neo4j");
// var uri = neo4jConfig["Uri"] ?? "bolt://localhost:7687";
// var user = neo4jConfig["User"] ?? "neo4j";
// var password = neo4jConfig["Password"] ?? "password";

// // --- 2. NEO4J DRIVER (Potreban za Initializer) ---
// builder.Services.AddSingleton(GraphDatabase.Driver(
//     uri, 
//     AuthTokens.Basic(user, password)
// ));

// // --- 3. NEO4J CLIENT (Fakultetski stil - IGraphClient) ---
// var client = new GraphClient(new Uri("http://localhost:7474"), user, password); // Neo4jClient koristi HTTP port 7474
// try 
// {
//     await client.ConnectAsync();
//     builder.Services.AddSingleton<IGraphClient>(client);
// }
// catch (Exception ex)
// {
//     Console.WriteLine($"Greška pri povezivanju na Neo4jClient: {ex.Message}");
// }

// // --- 4. REGISTRACIJA REPOZITORIJUMA ---
// builder.Services.AddScoped<CountryRepository>();
// builder.Services.AddScoped<CityRepository>();
// builder.Services.AddScoped<BillRepository>();
// builder.Services.AddScoped<FlightRepository>();
// builder.Services.AddScoped<RouteRepository>();
// builder.Services.AddScoped<UserRepository>();
// builder.Services.AddScoped<TicketRepository>();
// builder.Services.AddScoped<SuitcasesRepository>();
// builder.Services.AddScoped<BookingRepository>();
// builder.Services.AddScoped<FilterRepository>();
// // --- 5. INICIJALIZACIJA BAZE ---
// builder.Services.AddSingleton<Neo4jInitializer>();

// // --- 6. STANDARDNE SERVISNE KONFIGURACIJE ---
// builder.Services.AddControllers(); 
// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();

// var app = builder.Build();

// // --- 7. POKRETANJE INICIJALIZACIJE (Constraints/Indexes) ---
// using (var scope = app.Services.CreateScope())
// {
//     var initializer = scope.ServiceProvider.GetRequiredService<Neo4jInitializer>();
//     await initializer.InitializeAsync();
// }

// // --- 8. MIDDLEWARE (HTTP Pipeline) ---
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }

// app.UseHttpsRedirection();
// app.UseAuthorization();

// app.UseCors();

// // Mapiranje ruta za kontrolere (bitno za [Route("api/[controller]")])
// app.MapControllers(); 

// // Pomoćna ruta za proveru da li API radi
// app.MapGet("/", () => "FlyWise API is running...");

// app.Run();

using Neo4j.Driver;           
using Neo4jClient;           
using Api.Data;               
using Api.Repositories;       
using StackExchange.Redis;    
using Utilities;              
using RedisDataLayer;         
using Data.Redis;
using DotNetEnv;        

DotNetEnv.Env.Load(); 

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// --- 1. KONFIGURACIJA IZ .env ILI appsettings.json ---
var neo4jUri = Environment.GetEnvironmentVariable("NEO4J_URI") ?? "bolt://localhost:7687";
var neo4jUser = Environment.GetEnvironmentVariable("NEO4J_USER") ?? "neo4j";
var neo4jPassword = Environment.GetEnvironmentVariable("NEO4J_PASSWORD") ?? "password";
var redisConnection = Environment.GetEnvironmentVariable("REDIS_CONNECTION") ?? "localhost:6379";

Console.WriteLine($"Neo4j URI: {neo4jUri}");
Console.WriteLine($"Redis: {redisConnection}");



// --- 2. NEO4J DRIVER ---
try
{
    var driver = GraphDatabase.Driver(
        neo4jUri, 
        AuthTokens.Basic(neo4jUser, neo4jPassword)
    );
    builder.Services.AddSingleton(driver);
    Console.WriteLine("✅ Neo4j Driver je povezan");
}
catch (Exception ex)
{
    Console.WriteLine($"⚠️ Neo4j Driver greška: {ex.Message}");
}

// --- 3. NEO4J CLIENT (IGraphClient) ---
IGraphClient? graphClient = null;
try
{
    graphClient = new GraphClient(new Uri("http://localhost:8081"), neo4jUser, neo4jPassword);
    graphClient.ConnectAsync().Wait();
    builder.Services.AddSingleton<IGraphClient>(graphClient);
    Console.WriteLine("✅ Neo4j Client je povezan");
}
catch (Exception ex)
{
    Console.WriteLine($"⚠️ Neo4j Client greška: {ex.Message}");
    
    // ✅ KREIRAJ DUMMY IGraphClient AKO FAILA
    // Ovo omogućava aplikaciji da se pokrene čak i bez Neo4j
    builder.Services.AddSingleton<IGraphClient>(sp => 
    {
        throw new InvalidOperationException("Neo4j nije dostupan. Molimo pokrenite Neo4j server.");
    });
}

// --- 4. REDIS SETUP ---
try
{
    var redis = ConnectionMultiplexer.Connect(redisConnection);
    builder.Services.AddSingleton<IConnectionMultiplexer>(redis);
    Console.WriteLine("✅ Redis je povezan");
}
catch (Exception ex)
{
    Console.WriteLine($"Redis greška: {ex.Message}");
    throw; // Redis je OBAVEZNA, pa bacamo grešku
}

// --- 5. REGISTRACIJA REPOZITORIJUMA ---
// ⚠️ Samo registriraj ako je Neo4j dostupan
if (graphClient != null)
{
    builder.Services.AddScoped<CountryRepository>();
    builder.Services.AddScoped<CityRepository>();
    builder.Services.AddScoped<BillRepository>();
    builder.Services.AddScoped<FlightRepository>();
    builder.Services.AddScoped<RouteRepository>();
    builder.Services.AddScoped<UserRepository>();
    builder.Services.AddScoped<TicketRepository>();
    builder.Services.AddScoped<SuitcasesRepository>();
    builder.Services.AddScoped<FilterRepository>();
    builder.Services.AddScoped<BookingRepository>();
    Console.WriteLine("✅ Svi Repositories su registrirani");
}
else
{
    Console.WriteLine("⚠️ Repositories nisu registrirani (Neo4j nedostaje)");
}

// --- 6. AUTH SERVICES ---
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<RedisUserCache>();
builder.Services.AddScoped<RedisAuthManager>();
Console.WriteLine("✅ Auth Services su registrirani");

// --- 7. INICIJALIZACIJA BAZE ---
if (graphClient != null)
{
    builder.Services.AddSingleton<Neo4jInitializer>();
}

// --- 8. STANDARDNE SERVISNE KONFIGURACIJE ---
builder.Services.AddControllers(); 
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// --- 9. POKRETANJE INICIJALIZACIJE ---
if (graphClient != null)
{
    try
    {
        using (var scope = app.Services.CreateScope())
        {
            var initializer = scope.ServiceProvider.GetRequiredService<Neo4jInitializer>();
            await initializer.InitializeAsync();
            Console.WriteLine("✅ Neo4j Initializer je završio");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"⚠️ Neo4j Initializer greška: {ex.Message}");
    }
}

// --- 10. MIDDLEWARE ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();


app.MapControllers(); 

app.MapGet("/", () => "FlyWise API is running...");
app.MapGet("/health", () => new { status = "ok", neo4j = graphClient != null ? "connected" : "disconnected" });

app.Run();