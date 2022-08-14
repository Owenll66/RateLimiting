namespace RateLimiting
{
    public static class RateLimiterFactory
    {
        public static RateLimiter Create(RateLimiterType rateLimiterType, int maxRequestNum, int period)
        {
            switch (rateLimiterType)
            {
                case RateLimiterType.TokenBucket:
                    return new TokenBucketRateLimiter(maxRequestNum, period);

                case RateLimiterType.SlidingWindow:
                    return new SlidingWindowRateLimiter(maxRequestNum, period);

                case RateLimiterType.FixedWindow:
                    return new FixedWindowRateLimiter(maxRequestNum, period);

                default:
                    throw new ArgumentException("Rate Limiter Type is not valid", nameof(rateLimiterType));
            }
        }
    }

    public enum RateLimiterType
    {
        TokenBucket,
        SlidingWindow,
        FixedWindow
    }
}
