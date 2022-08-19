namespace RateLimiting
{
    public class TokenBucketRateLimiter : RateLimiter
    {
        private readonly double _initialToken;
        private readonly object _lockObj = new();

        private double _currentToken;
        private DateTime? _lastRequestedTime;

        /// <inheritdoc/>
        /// <param name="initialTokens">
        /// Set the number of the initialToken to 0 to avoid burst rate in the begining.
        /// </param>
        public TokenBucketRateLimiter(int maxRequestNum, int period, int initialTokens = 0)
            : base(maxRequestNum, period)
        {
            _initialToken = initialTokens;
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
                    // Lazy refilling - refill only when the request is received (expect for the first request).
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
