using System;
using System.Text;
using Sora.Framework.Enums;
using Sora.Framework.Objects;
using Sora.Framework.Objects.Scores;

namespace Sora.Framework.Utilities
{
    public static class ScoreSubmissionParser
    {
        private const string PrivateKey = "osu!-scoreburgr---------{0}";

        public static (bool Pass, Score score) ParseScore(string encScore, string iv,
            string osuVersion)
        {
            var decryptedScore = Crypto.DecryptString(
                Convert.FromBase64String(encScore),
                Encoding.ASCII.GetBytes(string.Format(PrivateKey, osuVersion)),
                Convert.FromBase64String(iv)
            );

            var x = decryptedScore.Split(':');
            var score = new Score
            {
                FileMd5 = x[0],
                UserName = x[1],
                Count300 = int.Parse(x[3]),
                Count100 = int.Parse(x[4]),
                Count50 = int.Parse(x[5]),
                CountGeki = int.Parse(x[6]),
                CountKatu = int.Parse(x[7]),
                CountMiss = int.Parse(x[8]),
                TotalScore = int.Parse(x[9]),
                MaxCombo = short.Parse(x[10]),
                Mods = (Mod) uint.Parse(x[13]),
                PlayMode = (PlayMode) byte.Parse(x[15]),
                Date = DateTime.Now
            };

            return (bool.Parse(x[14]), score);
        }
    }
}
