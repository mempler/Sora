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
            var pwHash = Crypto.BCrypt.generate_hash(pw);
            Assert.NotNull(pwHash);
            
            Assert.IsTrue(Crypto.BCrypt.validate_password(pw, pwHash));
            Assert.IsFalse(Crypto.BCrypt.validate_password(pw + "kaese", pwHash));
        }
    }
}