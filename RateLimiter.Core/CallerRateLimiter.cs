
using RateLimiter.Core.Models;
using RateLimiter.Core.Storage;
using RateLimiter.Core.Strategy;

namespace RateLimiter.Core
{
    public class CallerRateLimiter<TArg>
    {
        private readonly List<Policy> _policies;
        private readonly Func<TArg, Task> _callAction;
        private readonly ILimitStrategy _strategy;

        private readonly Record _record;
        private readonly SemaphoreSlim _semaphore;

        public CallerRateLimiter(List<Policy> policies, Func<TArg, Task> callAction, ILimitStrategy strategy)
        {
            _policies = policies;
            _callAction = callAction;
            _strategy = strategy;

            _record = new Record();
            _semaphore = new SemaphoreSlim(1, 1);
        }

        public async Task<bool> ExecuteRequest(DateTime reqTime, RequestPacket<TArg> request)
        {
            bool is_allowed = false;
            await _semaphore.WaitAsync();

            try
            {
                is_allowed = _strategy.IsAllowed(reqTime, _policies, _record));
            }
            finally
            {
                _semaphore.Release();
            }

            if (is_allowed)
            {
                await _callAction(request.Arg!);
            }

            return is_allowed;
        }
        
        public void logs(string id)
        {
            Console.WriteLine("=== Rate Limiter Statistics ===");
                Console.WriteLine($"Client {id}:");

                foreach (var policy in _policies)
                {
                    Console.WriteLine($"  {policy.UId}: {_record.GetCurrentLimit(policy.UId)} requests");
                }
            Console.WriteLine("==============================");
        }
    }
}