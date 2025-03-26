
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
            await _semaphore.WaitAsync();

            try
            {
                if (_strategy.IsAllowed(reqTime, _policies, _record))
                {
                    await _callAction(request.Arg!);
                }
            }
            finally
            {
                _semaphore.Release();
            }

            return false;
        }

        // public void LogRequestStats()
        // {
        //     foreach (var id in _records)
        //     {
        //         Console.WriteLine($"Request {id.Key}:");

        //         for (int i = 0; i < _policies.Count; i++)
        //         {
        //             Console.WriteLine($"Policy {i + 1} - Limit: {_policies[i].Limit}, Requests: {id.Value.Counters[i]}");
        //         }
        //     }
        // }
    }
}