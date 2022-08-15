using System;
using System.Threading;

using RateLimiting;

using NUnit.Framework;

namespace RateLimiterTest
{
    public class TokenBucketRateLimiterTests
    {
        private readonly RateLimiter _rateLimiter;

        public TokenBucketRateLimiterTests()
        {
            _rateLimiter = RateLimiterFactory.Create(RateLimiterType.TokenBucket, 10, 1);
        }

        [Test]
        public void Test_Rate_With_Constant_Requests()
        {
            // Arrange & Act
            var count = 0;
            var startTime = DateTime.Now;

            while ((DateTime.Now - startTime).TotalSeconds < 10)
            {
                if (_rateLimiter.IsAllow())
                    count++;
            }

            var seconds = (DateTime.Now - startTime).TotalMilliseconds / 1000;
            var rate = count / seconds;

            // The rate will be very close to 9.9. Since the initial token number is 0, and
            // One token will be refilled around every 0.1 second within total of 10 seconds.
            // (99 tokens / 10 seconds) is approximately 9.9 request/second.
            PrintTestResult(count, seconds);
            Assert.That(rate, Is.EqualTo(10).Within(0.2));
        }

        [Test]
        public void Test_Rate_With_Burst_Request()
        {
            // Arrange & Act
            var count = 0;
            var startTime = DateTime.Now;

            // Send first request to let the token bucket rate limiter start
            // to accumulate tokens.
            _rateLimiter.IsAllow();
            count++;

            // Sleep 5 seconds and there will be a burst requsts to consume
            // all the existing tokens after that.
            Thread.Sleep(5000);

            while ((DateTime.Now - startTime).TotalSeconds < 10)
            {
                if (_rateLimiter.IsAllow())
                    count++;
            }

            // The first 5 seconds didn't handle any request except the first request.
            // So we only take the last five seconds into account.
            var seconds = (DateTime.Now - startTime).TotalMilliseconds / 1000 - 5;
            var rate = count / seconds;

            /// This test is testing the limitation of TokenBucket algorithm. It cannot 
            /// handle burst request very well due to the nature of the algorithm.
            /// After 5 seconds, the bucket will be full (total 10 tokens). and 
            /// it will accumulate 1 every 0.1 seconds. So the total token consumed
            /// will be around (1 + 49 + 10) and allowed in the last 5 seconds. 
            /// So the rate will be very close to 60 requests / 5 seconds = 12 request / second.
            /// Which is over 10 requests / second.
            PrintTestResult(count, seconds);
            Assert.That(rate, Is.EqualTo(12).Within(0.2));
        }

        private void PrintTestResult(int count, double seconds)
        {
            Console.WriteLine($"{count} requests are allowed in {seconds} seconds.");
            Console.WriteLine($"Rate: {count / seconds}");
        }
    }
}