using NUnit.Framework;
using Sora.Framework.Allocation;

namespace Sora.Framework.Tests.Allocation
{
    public class DynamicValuesTests
    {
        private IDynamicValues values;
        
        public DynamicValuesTests() => values = new DynamicValues();

        [SetUp]
        public void Setup()
        {
            values.Set("SOME_KEY_SETTER", "Not Null");
        }
        
        [Test]
        public void TestValues()
        {
            Assert.IsTrue(values.TryGet("SOME_KEY_SETTER", out string val));
            Assert.NotNull(val);
            Assert.AreEqual(val, "Not Null");
        }
    }
}