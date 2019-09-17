using NUnit.Framework;
using Sora.Framework.Utilities;

namespace Sora.Framework.Tests.Utilities
{
    public class SCryptTests
    {
        [Test]
        public void TestSCrypt()
        {
            var pw = Crypto.RandomString(32);
            var (pwHash, salt) = Crypto.SCrypt.generate_hash(pw);

            Assert.IsTrue(Crypto.SCrypt.validate_password(pw, pwHash, salt));
        }
    }
}