namespace RateLimiter.Service.Interface
{
    public interface IRateLimiterService<TArg>
    {
        Task<bool> Perform(TArg arg);
    }
}