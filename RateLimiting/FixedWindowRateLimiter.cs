using System.Collections.Concurrent;

namespace RateLimiting
{
    public class FixedWindowRateLimiter : RateLimiter
    {
        // TODO: Need a logic to clean up the window.
        // key is the window start time. value is the number of requests accepted.
        private ConcurrentDictionary<long, int> _windowCounts = new();
        private object _lockObj = new();

        public FixedWindowRateLimiter(int maxRequestNum, int period)
            : base(maxRequestNum, period)
        {
        }

        public override bool IsAllow()
        {
            var window = DateTimeOffset.Now.ToUnixTimeMilliseconds() / (Period * 1000) * Period * 1000;

            if (_windowCounts.TryAdd(window, 1))
            {
                // If the key doesn't exists, it will add a new entry in the
                // dictionary and start counting the number of requests received in
                // that window frame.
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
