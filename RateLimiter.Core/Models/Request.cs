namespace RateLimiter.Core.Models
{
    public class Request<TArg>
    {
            public string Id { get; }
            public List<Policy> Policies { get; }
            public Func<TArg, Task> CallAction { get; }
            public TArg? Arg { get; }

            public Request(string id, List<Policy> polocies, Func<TArg, Task> callAction, TArg? arg)
            {
                Id = id;
                Policies = polocies;
                CallAction = callAction;
                Arg = arg;
            }
    }
}