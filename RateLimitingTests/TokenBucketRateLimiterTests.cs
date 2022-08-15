using NUnit.Framework;
using RateLimiting;

using System;

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
        public void Test_Rate_Single_Thread()
        {
            // Arrange & Act
            var count = 0;
            var startTime = DateTime.Now;

            while ((DateTime.Now - startTime).TotalSeconds < 10)
            {
                if (_rateLimiter.IsAllow())
                    count++;
            }

            var endTime = DateTime.Now;
            var rate = count / ((endTime - startTime).TotalMilliseconds / 1000);

            Console.WriteLine(rate);
            Assert.That(rate, Is.EqualTo(10).Within(0.2));
        }
    }
}