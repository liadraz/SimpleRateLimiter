using RateLimiter.Service.Models;

namespace RateLimiter.Service
{
    //  RateLimiter Service Interface
    //  
    //  The rate limiter service is the main moudle of this service. It is responsible to hold and pass requests each refers to unique caller
    //  Policies are being passed by the a Request class
    // 
    //      The Perform method is called by a caller to access an endpoint, 
    //
    public interface IRateLimiterService<TArg>
    {
        Task Perform(Request<TArg> request);
    }
}
                