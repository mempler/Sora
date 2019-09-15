using System;
using System.Threading;
using NUnit.Framework;
using Sora.Framework.Allocation;

namespace Sora.Framework.Tests.Allocation
{
    public class CacheTests
    {
        private readonly Cache _cache;
        
        public CacheTests() => _cache = Cache.New();

        [SetUp]
        public void Setup()
        {
            _cache.Set("Some Key", "Hello", TimeSpan.FromMilliseconds(50));
        }
        
        [Test]
        public void TestValues()
        {
            Assert.IsTrue(_cache.TryGet("Some Key", out string val));
            Assert.NotNull(val);
            Thread.Sleep(50);
            Assert.IsFalse(_cache.TryGet("Some Key", out val));
            Assert.Null(val);
        }
    }
}