using NUnit.Framework;
using Sora.Allocation;

namespace Sora.Tests.AllocationTests
{
    public class DynamicValuesTests
    {
        private IDynamicValues values;
        
        public DynamicValuesTests() => values = new DynamicValues();

        [Test]
        public void TestValues()
        {
            values.Set("SOME_KEY_SETTER", "Not Null");
            Assert.NotNull(values.Get<string>("SOME_KEY_SETTER"));
            Assert.AreEqual(values.Get<string>("SOME_KEY_SETTER"), "Not Null");
            Assert.Null(values.Get<string>("THIS_SHOULD_BE_NULL"));

            values["SOME_OTHER_KEY"] = "Also not null";
            Assert.Null(values["THIS_SHOULD_BE_NULL"]);
            Assert.NotNull((string) values["SOME_OTHER_KEY"]);
            Assert.AreEqual((string) values["SOME_OTHER_KEY"], "Also not null");
        }
    }
}