
using RateLimiter.Service;
using RateLimiter.Service.Models;

namespace RateLimiter.MainApp
{
    public class RateLimiterRun
    {
        public static async Task Main()
        {
            Console.WriteLine("\n~ Welcome to RateLimiter Service ~\n");
            
            var rateLimiter = new RateLimiterService<string>();
            
            string clientId = "Client01";
            Func<string?, Task> CallAction = (string? arg) => Task.Delay(TimeSpan.FromSeconds(1));
            List<Policy> policies = 
            new()
            {   
                new (1, TimeSpan.FromSeconds(3)),
                // new (5, TimeSpan.FromSeconds(10)),
                // new (5, TimeSpan.FromMinutes(1)),
                // new (10, TimeSpan.FromMinutes(10))
            };

            var request = new Request<string>(clientId, policies, CallAction, clientId);


            List<Task> tasks = new List<Task>();
            for (int i = 0; i < 10; i++)
            {
                tasks.Add(Task.Run(() => rateLimiter.Perform(request)));
            }

            await Task.WhenAll(tasks);

            Console.WriteLine("Press ENTER to stop");
            Console.ReadLine();
        }
    }
}
