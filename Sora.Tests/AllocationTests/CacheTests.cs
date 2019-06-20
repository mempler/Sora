using System;
using System.Threading;
using NUnit.Framework;
using Sora.Allocation;

namespace Sora.Tests.AllocationTests
{
    [TestFixture]
    public class CacheTests
    {
        private Cache _cache;
        
        public CacheTests()
        {
            _cache = Cache.New();
        }

        [Test]
        public void TestValues()
        {
            _cache.Set("Some Key", "Hello", TimeSpan.FromSeconds(1));
            Assert.NotNull(_cache.Get<string>("Some Key"));
            Thread.Sleep(1000);
            Assert.Null(_cache.Get<string>("Some Key"));
        }
    }
}