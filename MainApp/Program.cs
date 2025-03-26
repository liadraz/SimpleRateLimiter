
using RateLimiter.Service;
using RateLimiter.Core.Models;

namespace RateLimiter.MainApp
{
    public class RateLimiterRun
    {
        public static async Task Main()
        {
            // Create Caller Packets

            Func<string, Task> CallExternalApi = async (arg) =>
            {
                Console.WriteLine($"Request with argument {arg} is being executed.");
                await Task.Delay(TimeSpan.FromSeconds(3));
            };

            var policiesList = new List<Policy> 
            {
                new (10, TimeSpan.FromSeconds(1)),
                new (100, TimeSpan.FromMinutes(1)),
                new (1000, TimeSpan.FromMinutes(10))
            };
            
            var callReq = new RequestPacket<string>("Call01", policiesList, CallExternalApi, "8");

            // Create RateLimiter Service
            var rateLimiter = new RateLimiterService<string>();

            rateLimiter.Perform(callReq).Wait();

            //
            // Initate the RateLimiterService
            // Initaite muliple Clients each have unique ID
            
            // Each client will send requests randomly which will be processed by All of the RateLimiter policies.
            // The ratelimiter should be able to handle multiple clients having different IDs as each client will have its own counter.
            
            // Print Every Second Each client requests counter for all of the RateLimiterpolicies
            // Also print how many requests were allowed and how many were rejected.
        }

        public static async Task CallExternalApi(int arg)
        {
            // Simulate API call

            await Task.Delay(1000);
        }

    }
}