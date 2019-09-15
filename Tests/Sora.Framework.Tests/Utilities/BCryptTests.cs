using NUnit.Framework;
using Sora.Framework.Utilities;

namespace Sora.Framework.Tests.Utilities
{
    public class BCryptTests
    {
        [Test]
        public void TestBCrypt()
        {
            var pw = Crypto.RandomString(32); // Random Password with length of 32.
            Assert.NotNull(pw);
            var pwHash = BCrypt.generate_hash(pw);
            Assert.NotNull(pwHash);
            
            Assert.IsTrue(BCrypt.validate_password(pw, pwHash));
            Assert.IsFalse(BCrypt.validate_password(pw + "kaese", pwHash));
        }
    }
}