using NUnit.Framework;
using Sora.Allocation;

namespace Sora.Tests.AllocationTests
{
    [TestFixture]
    public class DynamicValuesTests
    {
        public IDynamicValues values;
        
        public DynamicValuesTests()
        {
            values = new DynamicValues();
        }
        
        [Test]
        public void TestValues()
        {
            values.Set("SOME_KEY_SETTER", "Not Null");
            Assert.IsNotNull(values.Get<string>("SOME_KEY_SETTER"));
            Assert.AreEqual(values.Get<string>("SOME_KEY_SETTER"), "Not Null");

            values["SOME_OTHER_KEY"] = "Also not null";
            Assert.IsNotNull((string) values["SOME_OTHER_KEY"]);
            Assert.AreEqual((string) values["SOME_OTHER_KEY"], "Also not null");
        }
    }
}