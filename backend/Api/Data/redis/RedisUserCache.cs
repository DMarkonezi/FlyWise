using StackExchange.Redis;
using System;
using System.Text.Json;
using Api.models.neo4j.Nodes;

namespace Data.Redis
{
    public class RedisUserCache
    {
        private readonly IConnectionMultiplexer _redis;

        public RedisUserCache(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        // Spremi user u cache za 7 dana
        public async Task<bool> CacheUserAsync(User user)
        {
            try
            {
                var db = _redis.GetDatabase();
                var key = $"user:{user.Email}";
                var json = JsonSerializer.Serialize(user);

                // Spremi sa TTL od 7 dana
                await db.StringSetAsync(key, json, TimeSpan.FromDays(7));
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Dohvati user iz cache-a
        public async Task<User?> GetCachedUserAsync(string email)
        {
            try
            {
                var db = _redis.GetDatabase();
                var key = $"user:{email}";

                var json = await db.StringGetAsync(key);

                if (!json.HasValue)
                    return null;

                return JsonSerializer.Deserialize<User>(json.ToString());
            }
            catch
            {
                return null;
            }
        }

        // Obriši user iz cache-a
        public async Task<bool> RemoveCachedUserAsync(string email)
        {
            try
            {
                var db = _redis.GetDatabase();
                var key = $"user:{email}";

                return await db.KeyDeleteAsync(key);
            }
            catch
            {
                return false;
            }
        }
    }
}