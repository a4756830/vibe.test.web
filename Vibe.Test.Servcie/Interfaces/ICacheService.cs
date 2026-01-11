using StackExchange.Redis;

namespace Vibe.Test.Servcie.Interfaces;

public interface ICacheService
{
    Task<bool> SetAsync(string key, string value, TimeSpan? expiry = null);
    Task<string?> GetAsync(string key);
    Task<bool> DeleteAsync(string key);
    Task<bool> ExistsAsync(string key);
}
