namespace RateLimiting
{
    public abstract class RateLimiter
    {
        protected int MaxRequestNum;
        protected int Period;

        /// <summary>
        /// Create a rate limiter provding the rate by maxRequestNum divided by a time period in seconds.
        /// </summary>
        /// <param name="maxRequestNum">The maximum request number within a period of time.</param>
        /// <param name="period">The period in seconds</param>
        public RateLimiter(int maxRequestNum, int period)
        {
            MaxRequestNum = maxRequestNum;
            Period = period;
        }

        public abstract bool IsAllow();
    }
}
