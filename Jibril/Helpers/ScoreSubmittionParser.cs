using System;
using System.Text;
using Shared.Enums;
using Shared.Helpers;
using Shared.Models;

namespace Jibril.Helpers
{
    public static class ScoreSubmittionParser
    {
        private const string PrivateKey = "osu!-scoreburgr---------{0}";
        public static (bool Pass, Scores score) ParseScore(string encscore, string iv, string osuversion)
        {
            string decryptedScore = Crypto.DecryptString(Convert.FromBase64String(encscore),
                                                         Encoding.ASCII.GetBytes(string.Format(PrivateKey, osuversion)),
                                                         Convert.FromBase64String(iv));
            string[] x = decryptedScore.Split(":");
            Scores score = new Scores
            {
                FileMd5 = x[0],
                UserId = Users.GetUserId(x[1]),
                Count300 = ulong.Parse(x[3]),
                Count100 = ulong.Parse(x[4]),
                Count50 = ulong.Parse(x[5]),
                CountGeki = ulong.Parse(x[6]),
                CountKatu = ulong.Parse(x[7]),
                CountMiss = ulong.Parse(x[8]),
                TotalScore = ulong.Parse(x[9]),
                MaxCombo = short.Parse(x[10]),
                Mods = (Mod) short.Parse(x[13]),
                PlayMode = (PlayMode) byte.Parse(x[15]),
                Date = DateTime.UtcNow,
            };
            score.Accuracy = Accuracy.GetAccuracy(score.Count300, score.Count100, score.Count50, score.CountGeki,
                                                  score.CountKatu, score.CountMiss, score.PlayMode);
            score.ScoreMd5 = Hex.ToHex(Crypto.GetMd5($"{score.Count300 + score.Count100}{score.FileMd5}{score.CountMiss}{score.CountGeki}{score.CountKatu}{score.Date}{score.Mods}"));
            score.ScoreOwner = Users.GetUser(score.UserId);
            
            return (bool.Parse(x[14]), score);
        }
    }
}