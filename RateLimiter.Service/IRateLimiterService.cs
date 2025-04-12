using RateLimiter.Core.Models;

namespace RateLimiter.Service
{
    //  RateLimiter Service Interface
    //  
    //  Creation of a rateLimiter service -
    //      The RateLimiter service allows the use of custom strategies 
    //      ensures all rate limit policies are honored.
    //      If no strategy is provided, the Sliding Window algorithm will be used by default.
    // 
    //      The Perform method is called by a caller to access an endpoint, 
    //      where the request is first processed by the rate limiter.
    //
    //      A call is made by passing a RequestPacket containing the following data:
    //          - callerID: The identifier of the caller making the request.
    //          - policies: A list of rate limit Policies to be enforced.
    //          - Func<TArg, Task>: A callback function executed when the request is allowed.
    //          - arg: A custom argument of type TArg that can be passed with the request.
    //
    public interface IRateLimiterService<TArg>
    {
        // Perform a rate-limited request by applying the defined policies and strategy.
        Task Perform(Request<TArg> request);
    }
}
