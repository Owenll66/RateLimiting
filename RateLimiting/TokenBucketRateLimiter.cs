namespace RateLimiting
{
    public class TokenBucketRateLimiter : RateLimiter
    {
        private double _currentToken;
        private double _initialToken;
        private DateTime? _lastRequestedTime;
        private object _lockObj = new object();

        public TokenBucketRateLimiter(int maxRequestNum, int period, int initialToken = 0)
            : base(maxRequestNum, period)
        {
            _initialToken = initialToken;
        }

        public override bool IsAllow()
        {
            var currentTime = DateTime.Now;

            lock (_lockObj)
            {
                // If it is the first time request. Set initial token(default is 0).
                if (_lastRequestedTime == null)
                {
                    _currentToken = _initialToken;
                }
                else
                {
                    var gap = currentTime - _lastRequestedTime;
                    var tokensToRefill = (gap.Value.TotalMilliseconds / 1000) * ((double)MaxRequestNum / Period);
                    _currentToken = Math.Min(_currentToken + tokensToRefill, MaxRequestNum);
                }

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
