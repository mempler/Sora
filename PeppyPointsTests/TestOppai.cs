using System.IO;
using NUnit.Framework;
using PeppyPoints;
using Shared.Enums;

namespace Tests
{
    public class TestOppai
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestPerformancePoints()
        {
            oppai op = new oppai("Beatmaps/Will Stetson - Snow Halation (feat. BeasttrollMC) (Sotarks) [NiNo's Insane].osu");

            op.SetMods(Mod.Hidden | Mod.DoubleTime);
            op.SetAcc(15, 10, 5);
            op.SetCombo(100);
            op.Calculate();
            
            Assert.GreaterOrEqual(op.GetPP(), 1);
        }
    }
}