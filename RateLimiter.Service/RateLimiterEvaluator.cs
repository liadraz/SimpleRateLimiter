
using RateLimiter.Service.Models;
using RateLimiter.Service.Storage;

namespace RateLimiter.Service
{
    public class RateLimiterEvaluator<TArg>
    {
        private readonly Record _record;
        private readonly List<Policy> _policies;
        private readonly SemaphoreSlim _semaphore = new(1, 1);

        public RateLimiterEvaluator(List<Policy> policies)
        {
            _record = new Record(policies);
            _policies = policies;
        }

        public async Task EvaluateAsync(DateTime reqTime, TArg arg, Func<TArg, Task> reqAction)
        {
            await _semaphore.WaitAsync();
            try
            {
                await WaitUntilAllowedAsync(reqTime);
                RegisterRequest(reqTime);
            }
            finally
            {
                _semaphore.Release();
            }

            await reqAction(arg);
        }

        private async Task WaitUntilAllowedAsync(DateTime reqTime)
        {
            foreach (var policy in _policies)
            {
                while (true)
                {
                    _record.CleanupExpired(reqTime, policy);
                    int validCount = _record.GetValidCount(reqTime, policy);

                    if (validCount < policy.Limit)
                    {
                        break;
                    }

                    var timestamps = _record.GetTimestamps(policy).ToList();
                    if (timestamps.Count == 0) 
                    {
                        break;
                    }

                    var first = timestamps.First();
                    var delay = (first + policy.WindowTime) - reqTime;

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
        }

        private void RegisterRequest(DateTime reqTime)
        {
            _record.Add(reqTime);
        }
    }
}