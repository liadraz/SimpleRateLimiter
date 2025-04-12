
using RateLimiter.Core.Models;
using RateLimiter.Core.Storage;
using RateLimiter.Core.Strategy;

namespace RateLimiter.Core
{
    public class CallerRateLimiter<TArg>
    {
        private readonly Record _record;
        private readonly ILimitStrategy _strategy;
        private readonly SemaphoreSlim _semaphore;

        public CallerRateLimiter(Request<TArg> request, ILimitStrategy strategy)
        {
            _strategy = strategy;

            _record = new Record();  
            _semaphore = new SemaphoreSlim(1, 1);
        }

        public async Task<bool> ExecuteRequest(DateTime reqTime, Request<TArg> request, List<Policy> rateLimiterPolicies)
        {
            await _semaphore.WaitAsync();

            try
            {
                while (!_strategy.IsAllowed(reqTime, rateLimiterPolicies, _record))
                {
                    var delay = _strategy.GetRequiredDelay(reqTime, rateLimiterPolicies, _record);

                    if (delay > TimeSpan.Zero)
                    {
                        await Task.Delay(delay);
                        reqTime = DateTime.UtcNow;
                    }
                    else
                    {
                        break;
                    }
                }

                _record.AddTimeStamp(reqTime);
            }
            finally
            {
                _semaphore.Release();
            }

            await request.CallAction(request.Arg!);

            return true;
        }
        

        public void logs(string id)
        {
            Console.WriteLine("=== Rate Limiter Statistics ===");
                Console.WriteLine($"Client {id}:");

                foreach (var policy in rateLimiterPolicies)
                {
                    Console.WriteLine($"  {policy.UId}: {_record.GetCurrentLimit(policy.UId)} requests");
                }
            Console.WriteLine("==============================");
        }
    }
}

// 1.
// delay -> all requests are allowed

// 2.
// In record each policy on its own queue                   V
// Enter to Add record / only strategy should be on caller  V
// to many classes knows record

// return excption

// work only with request
// to many files
// two problems - delay, and the ts per each policy

