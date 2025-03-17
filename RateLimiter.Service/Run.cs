using System;
using RateLimiter.Core.Models;
using RateLimiter.Core.Strategy;
class Run
{
    public static async Task Main()
    {
        // Decalre general Func to be called once the Caller request passed
        Func<string, Task> callAction = async (arg) =>
        {
            Console.WriteLine("Request Allowed");
            await Task.Delay(100);
        };

        // For simplicity we will provide the RateLimiter with a fixed list of policies 
            // A configuration file might be send to it, be parsed and built inside the RateLimiter. 
        var policies = new List<RateLimitPolicy> 
        {
            new RateLimitPolicy(10, TimeSpan.FromSeconds(1)),
            new RateLimitPolicy(100, TimeSpan.FromMinutes(1)),
            new RateLimitPolicy(100, TimeSpan.FromDays(1))
        };

        var slidingWindow = new SlidingWindowStrategy();
        
        //
        // Create the RateLimiterService
        var rateLimiter = new RateLimiterService<string>(callAction, slidingWindow, policies);

        // Simulate 100 requests from 5 clients
        var tasks = new List<Task>();
        for (int i = 0; i < 100; i++)
        {
            int reqNum = i + 1;
            string clientId = $"client_{reqNum % 5}";

            tasks.Add(rateLimiter.Perform($"Request {reqNum} from {clientId}", clientId));
        }

        await Task.WhenAll(tasks);
    }
}