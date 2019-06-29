using NUnit.Framework;
using Sora.Enums;
using Sora.Helpers;

namespace Sora.Tests.HelperTests
{
    public class AccuracyTests
    {
        [Test]
        public void OsuAccuracy()
        {
            const int C300 = 2038;
            const int C100 = 100;
            const int C50 = 2;
            const int CGeki = 239;
            const int CKatu = 49;
            const int CMiss = 5;
            
            var accStd = Accuracy.GetAccuracy(C300, C100, C50, CMiss, CGeki, CKatu, PlayMode.Osu);

            Assert.AreEqual(accStd, 0.9658119658119658);
        }

        [Test]
        public void TaikoAccuracy()
        {
            const int C300 = 870;
            const int C100 = 88;
            const int C50 = 0;
            const int CGeki = 0;
            const int CKatu = 0;
            const int CMiss = 1;
            
            var accTaiko = Accuracy.GetAccuracy(C300, C100, C50, CMiss, CGeki, CKatu, PlayMode.Taiko);
            
            Assert.AreEqual(accTaiko, 0.9530761209593326);
        }

        [Test]
        public void CtbAccuracy()
        {
            const int C300 = 3618;
            const int C100 = 28;
            const int C50 = 47;
            const int CGeki = 2768;
            const int CKatu = 1;
            const int CMiss = 2;
            
            var accCtb = Accuracy.GetAccuracy(C300, C100, C50, CMiss, CGeki, CKatu, PlayMode.Ctb);

            Logger.Info($"{accCtb}");
        }

        [Test]
        public void ManiaAccuracy()
        {
            const int C300 = 37;
            const int C100 = 24;
            const int C50 = 0;
            const int CGeki = 20;
            const int CKatu = 7;
            const int CMiss = 6;
            
            var accMania = Accuracy.GetAccuracy(C300, C100, C50, CMiss, CGeki, CKatu, PlayMode.Mania);

            Logger.Info($"{accMania:P}");
        }
    }
}
