
# Rate Limiter Assignment

## Motivation 
A mechanism that restricts the number of requests allowed to a specific endpoint. 
This is a valuable addition to enhancing security and ensuring quality control.

## Requirements
- Initialize the RateLimit service with a function, and allow the creation of multiple rate limits with varying request capacities and time windows.
- Provide a Perform method, Task Perform(TArg argument), as the primary interface for clients to interact with the service.
- Support multiple running rate limits, ensuring they are respected.
- Handles concurrent calls from multiple threads safely. 

> *NOTE Do not use external Libraries*

## Implementation Approaches

### Sliding Window
The algorithm ensures you are limited to several requests in any rolling 24-hour window.

*Pros*
- Prevents bursts near the reset times
- Requests are more scattered during the day
- More flexible as it doesn't reset at fixed intervals
- More precise in allowing rate limits based on the actual time of actions.
- No starvation problem

*Cons*
- Complex implementation
- Use more memory to track all timestamps (even the rejected).
- Handles timestamps for each execution.

#### Absolute
Ensures several requests are made on a calendar day starting at midnight (00:00).

*Pros*
- Predictable
- Easy to implement
*Cons*
- Can exceed the limit in a very short time or near reset times.
- Not accurate

## Project Layers

├───RateLimiter.Core
│   ├───Models
│   │       RateLimiterPolicy.cs
│   ├───Storage // Optional NOT IN USE
│   │       IRateLimitStorage.cs
│   │       localStorage.cs
│   └───Strategy
│           Absolute.cs // Optional NOT IN USE
│           IRateLimitStrategy.cs
│           SlidingWindow.cs
├───RateLimiter.Service
|   |   RateLimiterService.cs
│   │   Run.cs
└───RateLimiter.Tests // 
    │   UnitTest1.cs

## Usage

---

## *AsIs*
`Create a RateLimiter in C#.
It's purpose is to be initialized with some Func> and multiple(!) rate limits.

It is then called like so:
Task Perform(TArg argument) to perform the action. 
 
It guarantees that ALL rate limits it holds are honored, and delays execution until it can honor > the rate limits if needed. 
 
The RateLimiter IS expected to be called from multiple threads at the same time - you must > accommodate that. For example, the passed function might be a call to some external API, and it may> receive the rate limits: 10 per second, 100 per minute, 1000 per day
 
There are two approaches to Rate Limiting. Let's imagine a single RateLimit of 10 per day. The > approaches are:
1. Sliding window - Where the RateLimiter ensures that no more than 10 were executed in the last24 > hrs, even if the call is made in the middle of day
2. Absolute - Where the RateLimiter ensures that no more than 10 were executed THIS day, that is,> since 00:00.You need to choose the approach to implement, and explain why (pros / cons), and then> implement the RateLimiter.
 
Do not use or consult external libraries - YOU need to implement this.`
