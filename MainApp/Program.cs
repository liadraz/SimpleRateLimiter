
using RateLimiter.Service;
using RateLimiter.Service.Models;

namespace RateLimiter.MainApp
{
    public class RateLimiterRun
    {
        public static async Task Main()
        {
            Console.WriteLine("\n~ Welcome to RateLimiter Service ~\n");
            
            Func<string, Task> CallExternalApi = async (arg) =>
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
            };

            string clientId = "Client01";

            var request = new Request<string>(
            clientId, 
            Policy.RateLimiterPolicies,
            CallExternalApi,
            clientId);

            var rateLimiter = new RateLimiterService<string>();

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
