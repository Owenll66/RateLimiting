using System.Collections.Concurrent;

namespace Playground
{
    public class FixedWindowRateLimiter : RateLimiter
    {
        // key is the window start time. value is the number of requests accepted.
        private ConcurrentDictionary<long, int> _windowCounts = new ConcurrentDictionary<long, int>();
        private object _lockObj = new object();

        public FixedWindowRateLimiter(int maxRequestNum, int period)
            : base(maxRequestNum, period)
        {
        }

        public override bool IsAllow()
        {
            var window = DateTimeOffset.Now.ToUnixTimeMilliseconds() / (Period * 1000) * Period * 1000;

            if (_windowCounts.TryAdd(window, 1))
            {
                return true;
            }
            else
            {
                lock (_lockObj)
                {
                    if (_windowCounts[window] >= MaxRequestNum)
                    {
                        return false;
                    }
                    else
                    {
                        _windowCounts[window]++;
                        return true;
                    }
                }
            }
        }
    }
}
