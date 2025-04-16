﻿using System.Collections.Concurrent;
using RateLimiter.Service.Models;

namespace RateLimiter.Service
{
    public class RateLimiterService<TArg> : IRateLimiterService<TArg>
    {
        private readonly ConcurrentDictionary<string, RateLimiterEvaluator<TArg>> _evaluators = new();

        public async Task Perform(Request<TArg> request)
        {
            var reqTime = DateTime.UtcNow;
            var evaluator = _evaluators.GetOrAdd(
                request.Id,
                _ => new RateLimiterEvaluator<TArg>(request.Policies)
            );

            Console.WriteLine($"Request from {request.Arg} starts {reqTime:HH:mm:ss.fff}");

            await evaluator.EvaluateAsync(reqTime, request.Arg!, (arg) => request.CallAction(arg));

            Console.WriteLine($"Request from {request.Arg} Passed in {DateTime.UtcNow:HH:mm:ss.fff}\n\n");
        }
    }
}