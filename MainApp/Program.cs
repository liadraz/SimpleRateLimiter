
using RateLimiter.Service;
using RateLimiter.Core.Models;

namespace RateLimiter.MainApp
{
    public class RateLimiterRun
    {
        public static async Task Main()
        {
            Console.WriteLine("~ Welcome to RateLimiter Service ~");
            
            //
            // Create the Caller Packets Information
            Func<string, Task> CallExternalApi = async (arg) =>
            {
                Console.WriteLine($"Request Passed");
                await Task.Delay(TimeSpan.FromSeconds(2));
            };

            var policies = new List<Policy> 
            {
                new (1, TimeSpan.FromSeconds(1)),
                new (10, TimeSpan.FromMinutes(1)),
                new (1000, TimeSpan.FromMinutes(10))
            };
            
            var requests= new List<RequestPacket<string>>();
            for (int i = 0; i < 5; i++)
            {
                int clientID = i+1;

                requests.Add(new RequestPacket<string>(
                clientID.ToString(), 
                policies,
                CallExternalApi,
                "used by the callback"));
            }

            Console.WriteLine("Init the RateLimiter Service");
            var rateLimiter = new RateLimiterService<string>();

            // Token to control stopping
            CancellationTokenSource cts = new CancellationTokenSource();
            Random random = new Random();

            var requestTasks = new List<Task>();

            // Simulate random request calls from different clients
            var requestTask = Task.Run(async () =>
            {
                while (!cts.Token.IsCancellationRequested)
                {
                    var requestPacket = requests[random.Next(requests.Count)];

                    // Store the task so we can monitor it
                    var task = rateLimiter.Perform(requestPacket);
                    requestTasks.Add(task);
                    await task.ConfigureAwait(false);

                    // Random delay between 100-500ms
                    await Task.Delay(random.Next(100, 500));
                }
            });

            // Log statistics every second
            var logTask = Task.Run(async () =>
            {
                while (!cts.Token.IsCancellationRequested)
                {
                    rateLimiter.PrintStatistics();
                    await Task.Delay(1000);
                }
            });

            Console.WriteLine("Press ENTER to stop");
            Console.ReadLine();
            cts.Cancel();

            await Task.WhenAll(requestTask, logTask);
        }
    }
}

// Use Case Steps
//      Initate the RateLimiterService
//      Initaite muliple RequestPacket

//      Invoke Perform method, on a different times and from different clients.

//      Print Every second a log with each caller request ID and the counters for each policy