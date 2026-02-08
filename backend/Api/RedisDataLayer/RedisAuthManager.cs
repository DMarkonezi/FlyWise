// using StackExchange.Redis;
// using System;
// using System.Text.Json;
// using Api.Models.Auth;
// using Api.Models.Redis;
// using Api.Repositories;
// using Api.models.neo4j.Nodes;
// using Utilities;
// using Data.Redis;
// using BCrypt.Net;

// namespace RedisDataLayer
// {
//     public class RedisAuthManager
//     {
//         private readonly IConnectionMultiplexer _redis;
//         private readonly JwtService _jwtService;
//         private readonly UserRepository _userRepo;
//         private readonly RedisUserCache _userCache;

//         // Trebas DI - ili kreiraj direktno
//         public RedisAuthManager(
//             IConnectionMultiplexer redis, 
//             UserRepository userRepo,
//             JwtService jwtService)
//         {
//             try
//             {
//                 _redis = redis;
//                 _userRepo = userRepo;
//                 _jwtService = jwtService;
//                 _userCache = new RedisUserCache(redis);
//             }
//             catch (Exception ex)
//             {
//                 throw new Exception("Redis connection failed: " + ex.Message);
//             }
//         }

//         // LOGIN SA NEO4J + REDIS CACHE
//         public async Task<AuthResponse> LoginAsync(string email, string password)
//         {
//             try
//             {
//                 if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
//                     throw new Exception("Email and password required");

//                 // 1. Provjeri cache
//                 var user = await _userCache.GetCachedUserAsync(email);

//                 // 2. Ako nema u cache → Neo4j
//                 if (user == null)
//                 {
//                     user = await _userRepo.GetByEmailAsync(email);

//                     if (user == null)
//                         throw new Exception("User not found");

//                     // 3. Spremi u cache za sljedeći put
//                     await _userCache.CacheUserAsync(user);
//                 }

//                 // 4. Provjeri password (trebas BCrypt)
//                 if (!VerifyPassword(password, user.PasswordHash))
//                     throw new Exception("Invalid password");

//                 // 5. Generiraj token
//                 var token = _jwtService.GenerateToken(user.Id.ToString(), user.Email);

//                 // 6. Spremi session u Redis
//                 await SaveSessionInRedisAsync(user.Id.ToString(), token, user.Email);

//                 // 7. Vrati response
//                 return new AuthResponse
//                 {
//                     Token = token,
//                     User = new UserDto
//                     {
//                         Id = user.Id.ToString(),
//                         Email = user.Email,
//                         Name = user.Name
//                     },
//                     ExpiresIn = 86400
//                 };
//             }
//             catch (Exception ex)
//             {
//                 throw new Exception("Login failed: " + ex.Message);
//             }
//         }

//         // LOGOUT
//         public async Task LogoutAsync(string userId)
//         {
//             try
//             {
//                 var db = _redis.GetDatabase();
//                 var key = $"session:{userId}";
//                 await db.KeyDeleteAsync(key);
//             }
//             catch (Exception ex)
//             {
//                 throw new Exception("Logout failed: " + ex.Message);
//             }
//         }

//         // SPREMI SESSION U REDIS
//         private async Task SaveSessionInRedisAsync(string userId, string token, string email)
//         {
//             var db = _redis.GetDatabase();
//             var key = $"session:{userId}";

//             var sessionData = new
//             {
//                 Token = token,
//                 UserId = userId,
//                 Email = email,
//                 LoginTime = DateTime.UtcNow.ToString("O"),
//                 LastActivity = DateTime.UtcNow.ToString("O")
//             };

//             var json = JsonSerializer.Serialize(sessionData);

//             await db.StringSetAsync(key, json, TimeSpan.FromHours(24));
//         }

//         // DOHVATI SESSION
//         public async Task<SessionData?> GetSessionAsync(string userId)
//         {
//             try
//             {
//                 var db = _redis.GetDatabase();
//                 var key = $"session:{userId}";

//                 var json = await db.StringGetAsync(key);

//                 if (!json.HasValue)
//                     return null;

//                 return JsonSerializer.Deserialize<SessionData>(json.ToString());
//             }
//             catch
//             {
//                 return null;
//             }
//         }

//         // VALIDACIJA TOKENA
//         public async Task<bool> ValidateTokenAsync(string userId, string token)
//         {
//             try
//             {
//                 var session = await GetSessionAsync(userId);
//                 if (session == null)
//                     return false;

//                 if (session.Token != token)
//                     return false;

//                 return true;
//             }
//             catch
//             {
//                 return false;
//             }
//         }

//         private bool VerifyPassword(string password, string hash)
//         {
//             try
//             {
//                 return BCrypt.Net.BCrypt.Verify(password, hash);
//             }
//             catch
//             {
//                 return false;
//             }
//         }
//     }
// }

using StackExchange.Redis;
using System;
using System.Text.Json;
using Api.Models.Auth;
using Api.Models.Redis;
using Api.Repositories;
using Api.models.neo4j.Nodes;
using Utilities;
using Data.Redis;
using BCrypt.Net;

namespace RedisDataLayer
{
    public class RedisAuthManager
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly JwtService _jwtService;
        private readonly UserRepository _userRepo;
        private readonly RedisUserCache _userCache;

        public RedisAuthManager(
            IConnectionMultiplexer redis, 
            UserRepository userRepo,
            JwtService jwtService)
        {
            try
            {
                _redis = redis;
                _userRepo = userRepo;
                _jwtService = jwtService;
                _userCache = new RedisUserCache(redis);
            }
            catch (Exception ex)
            {
                throw new Exception("Redis connection failed: " + ex.Message);
            }
        }

        // LOGIN SA NEO4J + REDIS CACHE
        public async Task<AuthResponse> LoginAsync(string email, string password)
        {
            try
            {
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                    throw new Exception("Email and password required");

                Console.WriteLine($"Received login request: Email={email}, Password=***");

                // 1. Provjeri cache
                var user = await _userCache.GetCachedUserAsync(email);

                // 2. Ako nema u cache → Neo4j
                if (user == null)
                {
                    Console.WriteLine("User not found in cache. Fetching from Neo4j...");
                    user = await _userRepo.GetByEmailAsync(email);

                    if (user == null)
                        throw new Exception("User not found in DB");

                    // 3. Spremi u cache za sljedeći put
                    await _userCache.CacheUserAsync(user);
                }

                Console.WriteLine($"User found: Email={user.Email}, PasswordHash={(string.IsNullOrEmpty(user.PasswordHash) ? "NULL" : "***")}");

                // 4. Provjeri password
                bool passwordValid = VerifyPassword(password, user.PasswordHash);
                Console.WriteLine($"Password valid? {passwordValid}");

                if (!passwordValid)
                    throw new Exception("Invalid password");

                // 5. Generiraj token
                var token = _jwtService.GenerateToken(user.Id.ToString(), user.Email);

                // 6. Spremi session u Redis
                await SaveSessionInRedisAsync(user.Id.ToString(), token, user.Email);

                // 7. Vrati response
                return new AuthResponse
                {
                    Token = token,
                    User = new UserDto
                    {
                        Id = user.Id.ToString(),
                        Email = user.Email,
                        Name = user.Name
                    },
                    ExpiresIn = 86400
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login failed: {ex.Message}");
                throw new Exception("Login failed: " + ex.Message);
            }
        }

        // LOGOUT
        public async Task LogoutAsync(string userId)
        {
            try
            {
                var db = _redis.GetDatabase();
                var key = $"session:{userId}";
                await db.KeyDeleteAsync(key);
            }
            catch (Exception ex)
            {
                throw new Exception("Logout failed: " + ex.Message);
            }
        }

        private async Task SaveSessionInRedisAsync(string userId, string token, string email)
        {
            var db = _redis.GetDatabase();
            var key = $"session:{userId}";

            var sessionData = new
            {
                Token = token,
                UserId = userId,
                Email = email,
                LoginTime = DateTime.UtcNow.ToString("O"),
                LastActivity = DateTime.UtcNow.ToString("O")
            };

            var json = JsonSerializer.Serialize(sessionData);
            await db.StringSetAsync(key, json, TimeSpan.FromHours(24));
        }

        public async Task<SessionData?> GetSessionAsync(string userId)
        {
            try
            {
                var db = _redis.GetDatabase();
                var key = $"session:{userId}";

                var json = await db.StringGetAsync(key);
                if (!json.HasValue)
                    return null;

                return JsonSerializer.Deserialize<SessionData>(json.ToString());
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> ValidateTokenAsync(string userId, string token)
        {
            try
            {
                var session = await GetSessionAsync(userId);
                if (session == null)
                    return false;

                if (session.Token != token)
                    return false;

                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool VerifyPassword(string password, string hash)
        {
            if (string.IsNullOrEmpty(hash))
            {
                Console.WriteLine("Password hash is null or empty!");
                return false;
            }

            try
            {
                return BCrypt.Net.BCrypt.Verify(password, hash);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"BCrypt verify failed: {ex.Message}");
                return false;
            }
        }
    }
}
