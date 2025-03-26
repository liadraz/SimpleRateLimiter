using RateLimiter.Core.Models;

namespace RateLimiter.Service
{
    //
    //  This is the interface for the simple implemented rate limiter service
    //  
    //  Create A RateLimiter Service -
    //      The RateLimiter service allows the use of custom strategies 
    //      ensures all rate limit policies are honored.
    //      If no strategy is provided, the Sliding Window algorithm will be used by default.
    // 
    //      RequestPacket is a custom class representing a request. 
    //      It contains:
    //          - callerID: The identifier of the caller making the request.
    //          - policies: A list of rate limit Policies to be enforced.
    //          - Func<TArg, Task>: A callback function executed when the request is allowed.
    //          - arg: A custom argument of type TArg that can be passed with the request.
    //
    public interface IRateLimiterService<TArg>
    {
        // Perform a rate-limited request by applying the defined policies and strategy.
        Task Perform(RequestPacket<TArg> request);
    }
}
