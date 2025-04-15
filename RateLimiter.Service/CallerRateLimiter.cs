
using RateLimiter.Service.Models;
using RateLimiter.Service.Storage;
using RateLimiter.Service.Strategy;

namespace RateLimiter.Service
{
    public class CallerRateLimiter<TArg>
    {
        private readonly Record _record;
        private readonly SemaphoreSlim _semaphore;

        public CallerRateLimiter()
        {
            _record = new Record();
            _semaphore = new SemaphoreSlim(1, 1);
        }

        public async Task ExecuteRequest(DateTime reqTime, Request<TArg> request, List<Policy> rateLimiterPolicies, ILimitStrategy strategy)
        {
            await _semaphore.WaitAsync();

            try
            {
                foreach (var policy in rateLimiterPolicies)
                {
                    while (!strategy.IsRequestAllowed(reqTime, policy, _record))
                    {
                        DateTime? last = _record.GetLastTime(policy);
                        if (last == null)
                        {
                            break;
                        }
                        
                        TimeSpan delay = reqTime - last.Value;
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
                }

                _record.AddRecord(reqTime);
            }
            finally
            {
                _semaphore.Release();
            }

            await request.CallAction(request.Arg!);
        }
        

        // public void logs(string id)
        // {
        //     Console.WriteLine("=== Rate Limiter Statistics ===");
        //         Console.WriteLine($"Client {id}:");

        //         foreach (var policy in rateLimiterPolicies)
        //         {
        //             Console.WriteLine($"  {policy.UId}: {_record.GetRecordAmount(policy.UId)} requests");
        //         }
        //     Console.WriteLine("==============================");
        // }
    }
}
