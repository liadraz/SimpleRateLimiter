using System.Collections.Concurrent;
using RateLimiter.Core.Models;

namespace RateLimiter.Core.Storage
{
    public class RateLimitLocalStorage : IRateLimitStorage
    {
    private readonly ConcurrentDictionary<string, List<DateTime>> _requestLog = new();

    public Task<bool> TryConsumeAsync(string id, RateLimitPolicy policy)
    {
        var now = DateTime.UtcNow;
        var windowStart = now - policy.Window;

        _requestLog.AddOrUpdate(id, new List<DateTime> { now }, (key, list) =>
        {
            list.RemoveAll(timestamp => timestamp < windowStart);
            if (list.Count < policy.Limit)
            {
                list.Add(now);
                return list;
            }
            return list;
        });

        return Task.FromResult(_requestLog[id].Count <= policy.Limit);
        }
    }
}