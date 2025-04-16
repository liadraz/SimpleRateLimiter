
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

        public async Task EvaluateAsync(DateTime reqTime, Func<TArg?, Task> reqAction, TArg? arg)
        {
            Console.WriteLine($"Request starts {reqTime:HH:mm:ss.fff}");
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

            Console.WriteLine($"Request Passed {DateTime.UtcNow:HH:mm:ss.fff}\n\n");
            await reqAction(arg);
        }

        private async Task WaitUntilAllowedAsync(DateTime reqTime)
        {
            foreach (var policy in _policies)
            {
                while (true)
                {
                    _record.CleanupExpired(reqTime, policy);
                    if (_record.GetCount(policy) < policy.Limit)
                    {
                        break;
                    }

                    DateTime? first = _record.GetFirstValidTimestamp(policy);
                    if (first == null)
                    {
                        break;
                    }

                    var delay = (first + policy.WindowTime) - reqTime;
                    if (delay > TimeSpan.Zero)
                    {
                        await Task.Delay(delay.Value);
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