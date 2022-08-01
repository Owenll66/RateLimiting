namespace Playground
{
    public class TokenBucketRateLimiter : RateLimiter
    {
        private double _currentToken;
        private DateTime _lastRequestedTime;
        private object _lockObj = new object();

        public TokenBucketRateLimiter(int maxRequestNum, int period)
            : base(maxRequestNum, period)
        {
        }

        public override bool IsAllow()
        {
            var currentTime = DateTime.Now;

            lock (_lockObj)
            {
                var gap = currentTime - _lastRequestedTime;
                var tokensToRefill = (gap.TotalMilliseconds / 1000) * ((double)MaxRequestNum / Period);
                _currentToken = Math.Min(_currentToken + tokensToRefill, MaxRequestNum);
                 _lastRequestedTime = currentTime;

                if (_currentToken < 1)
                {
                    return false;
                }
                else
                {
                    _currentToken--;
                    return true;
                }
            }
        }
    }
}
