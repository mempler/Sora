using System;
using Sora.Framework.Enums;
using Sora.Framework.Utilities;

namespace Sora.Framework.Objects.Scores
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

        public int Id; // ID of Score, Required for Scoreboard.
        public int? ReplayId; // ID which the Replay has been saved! Required for Scoreboard
        public int UserId; // Required for Scoreboard.
        public int Position; // Position in Scoreboard.

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

        public string ToOsuString()
        {
            return $"{Id}|" +
                   $"{UserName.Replace("|", "I")}|" +
                   $"{TotalScore}|" +
                   $"{MaxCombo}|" +
                   $"{Count50}|" +
                   $"{Count100}|" +
                   $"{Count300}|" +
                   $"{CountMiss}|" +
                   $"{CountGeki}|" +
                   $"{CountKatu}|" +
                   $"{CountMiss > 0}|" +
                   $"{(short) Mods}|" +
                   $"{UserId}|" +
                   $"{Position}|" +
                   $"{(int) Date.ToUniversalTime().Subtract(new DateTime(1970, 1, 1)).TotalSeconds}|" +
                   $"{ReplayId ?? 0}";
        }
    }
}