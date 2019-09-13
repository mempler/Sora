using System;
using Sora.Framework.Enums;
using Sora.Framework.Utilities;

namespace Sora.Framework.Objects
{
    public class Score
    {
        public string FileMd5;
        public string UserName;
        public int Count300;
        public int Count100;
        public int Count50;
        public int CountGeki;
        public int CountKatu;
        public int CountMiss;
        public int TotalScore;
        public short MaxCombo;
        public Mod Mods;
        public PlayMode PlayMode;
        public DateTime Date;

        public double ComputeAccuracy()
        {
            var totalHits = Count50 + Count100 + Count300 + CountMiss;

            if (PlayMode == PlayMode.Mania)
                totalHits += CountGeki + CountKatu;
            
            switch (PlayMode)
            {
                case PlayMode.Osu:
                    return
                        totalHits > 0
                            ? (double) (Count50 * 50 + Count100 * 100 + Count300 * 300) /
                              (totalHits * 300)
                            : 1;
                case PlayMode.Taiko:
                    return
                        totalHits > 0
                            ? (double) (Count100 * 150 + Count300 * 300) /
                              (totalHits * 300)
                            : 1;
                case PlayMode.Ctb:
                    return
                        totalHits > 0
                            ? (double) (Count50 + Count100 + Count300) /
                              totalHits
                            : 1;
                case PlayMode.Mania:
                    return
                        totalHits > 0
                            ? (double) (Count50 * 50 + Count100 * 100 + CountKatu * 200 +
                                        (Count300 + CountGeki) * 300) /
                              (totalHits * 300)
                            : 1;
                default:
                    return 0;
            }
        }
        
        public string Checksum => Hex.ToHex(
            Crypto.GetMd5(
                $"{Count300 + Count100}{FileMd5}{CountMiss}{CountGeki}{CountKatu}{Date}{Mods}"
            )
        );
    }
}