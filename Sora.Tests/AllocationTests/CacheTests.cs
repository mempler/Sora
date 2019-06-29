using System;
using System.Threading;
using NUnit.Framework;
using Sora.Allocation;

namespace Sora.Tests.AllocationTests
{
    public class CacheTests
    {
        private readonly Cache _cache;
        
        public CacheTests() => _cache = Cache.New();

        [Test]
        public void TestValues()
        {
            _cache.Set("Some Key", "Hello", TimeSpan.FromMilliseconds(50));
            Assert.NotNull(_cache.Get<string>("Some Key"));
            Thread.Sleep(50);
            Assert.Null(_cache.Get<string>("Some Key"));
        }
    }
}