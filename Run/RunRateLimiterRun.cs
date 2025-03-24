using RateLimiter.Core.Models;
using RateLimiter.Core.Strategy;
using RateLimiter.Core.Service;

class RateLimiterRun
{
    public static async Task Main()
    {
        // Decalre general Func to be called once the Caller request passed
        Func<Task> callAction = async () =>
        {
            Console.WriteLine($"Request allowed");
            await Task.Delay(TimeSpan.FromSeconds(1));
        };


        // For simplicity we will provide the RateLimiter with a fixed list of policies 
        var policies = new List<RateLimitPolicy> 
        {
            new RateLimitPolicy(10, TimeSpan.FromSeconds(1)),
            new RateLimitPolicy(100, TimeSpan.FromMinutes(1)),
            new RateLimitPolicy(1000, TimeSpan.FromMinutes(10))
        };
        
        // Should be part of an stratgy dp. TODO cretae an interface for it
        var slidingWindow = new SlidingWindowStrategy();

        //
        // Create the RateLimiterService.
        var rateLimiter = new RateLimiterService<string>(callAction, policies, slidingWindow);

        //
        // Initate the RateLimiterService
        // Initaite muliple Clients each have unique ID
        
        // Each client will send requests randomly which will be processed by All of the RateLimiter policies.
        // The ratelimiter should be able to handle multiple clients having different IDs as each client will have its own counter.
        
        // Print Every Second Each client requests counter for all of the RateLimiterpolicies
        // Also print how many requests were allowed and how many were rejected.

        List<string> clients = new List<string> { "Potato", "Biscuit", "Lucky", "KipKip" };

        // Run continously the RateLimiterService and listen for Client requests
        var clientTasks = clients.Select(client => Task.Run(async ()=>
        {
            while (true)
            {
                // Simulate a random delay between requests
                await Task.Delay(new Random().Next(100, 500));

                await rateLimiter.Perform(client);
            }
        })).ToList();


        var logTask = Task.Run(async () =>
        {
            while (true)
            {
                rateLimiter.LogRequestStats();
                await Task.Delay(1000); // Log every second
            }
        });

        await Task.Delay(TimeSpan.FromMinutes(5));

        await Task.WhenAll(clientTasks);
        await logTask;

    }

}