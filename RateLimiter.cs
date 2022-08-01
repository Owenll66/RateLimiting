namespace Playground
{
    public abstract class RateLimiter
    {
        protected int MaxRequestNum;
        protected int Period;

        public RateLimiter(int maxRequestNum, int period)
        {
            MaxRequestNum = maxRequestNum;
            Period = period;
        }

        public abstract bool IsAllow();
    }
}
