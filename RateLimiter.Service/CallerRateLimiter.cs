
using RateLimiter.Service.Models;
using RateLimiter.Service.Storage;
using RateLimiter.Service.Strategy;

namespace RateLimiter.Service
{
    public class CallerRateLimiter<TArg>
    {
        private readonly Record _record;
        private readonly SemaphoreSlim _semaphore;

        public CallerRateLimiter(List<Policy> rateLimiterPolicies)
        {
            _record = new Record(rateLimiterPolicies);
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
                        
                        // TimeSpan delay = reqTime - last.Value;
                        TimeSpan delay = (last.Value + policy.WindowTime) - reqTime;
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
    }
}
