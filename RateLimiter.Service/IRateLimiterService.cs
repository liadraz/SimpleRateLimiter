using RateLimiter.Core.Models;

namespace RateLimiter.Service
{
    //  RateLimiter Service Interface
    //  
    //  The rate limiter service is the main unit of this service. It is responsible to hold the policies it suppose to restricts
    //  the strategy it uses to force reqiests and ther clients that calls it. Each client will have its own data Ratelimiter that holds the record data and handles the provided requests.
    //
    //  Creation of a rateLimiter service -
    //      By default it uses the Sliding Window algorithm.
    //      By default it uses a fixed LUT of policies
    // 
    //      The Perform method is called by a caller to access an endpoint, 
    //      where the request is first processed by the rate limiter.
    //
    //      A call is made by passing a RequestPacket containing the following data:
    //
    public interface IRateLimiterService<TArg>
    {
        Task Perform(Request<TArg> request);
    }
}
                
