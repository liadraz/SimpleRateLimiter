using RateLimiter.Service.Models;

namespace RateLimiter.Service
{
    //  RateLimiterService Interface
    //
    //  The IRateLimiterService interface is responsible for managing concurrent requests from several callers.
    //  It evaluates and process requests based on defined rate-limiting policies.
    //
    //  Usage
    //      Instantiate the rate limiter service (no arguments required).
    //      Use the Perform method to process a request, passing in a Request object.
    //      The Request class contains the specific policies that govern rate-limiting for the caller.
    //
    public interface IRateLimiterService<TArg>
    {
        Task Perform(Request<TArg> request);
    }
}
                