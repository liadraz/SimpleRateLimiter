using System;
using System;
using RateLimiter.Core.Models;

namespace RateLimiter.Core.Storage
{
    // Interface for rate-limit storage
    public interface IRateLimitStorage
    {
        Task<bool> TryConsumeAsync(string id, RateLimitPolicy policy);
    }
}