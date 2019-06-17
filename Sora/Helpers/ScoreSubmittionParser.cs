#region LICENSE
/*
    Sora - A Modular Bancho written in C#
    Copyright (C) 2019 Robin A. P.

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Affero General Public License as
    published by the Free Software Foundation, either version 3 of the
    License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Affero General Public License for more details.

    You should have received a copy of the GNU Affero General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/
#endregion

using System;
using System.Text;
using Sora.Database;
using Sora.Database.Models;
using Sora.Enums;

namespace Sora.Helpers
{
    public static class ScoreSubmittionParser
    {
        private const string PrivateKey = "osu!-scoreburgr---------{0}";
        public static (bool Pass, Scores score) ParseScore(SoraDbContextFactory factory, string encscore, string iv, string osuversion)
        {
            string decryptedScore = Crypto.DecryptString(Convert.FromBase64String(encscore),
                                                         Encoding.ASCII.GetBytes(string.Format(PrivateKey, osuversion)),
                                                         Convert.FromBase64String(iv));
            
            string[] x = decryptedScore.Split(":");
            Scores score = new Scores
            {
                FileMd5 = x[0],
                UserId = Users.GetUserId(factory, x[1]),
                Count300 = ulong.Parse(x[3]),
                Count100 = ulong.Parse(x[4]),
                Count50 = ulong.Parse(x[5]),
                CountGeki = ulong.Parse(x[6]),
                CountKatu = ulong.Parse(x[7]),
                CountMiss = ulong.Parse(x[8]),
                TotalScore = ulong.Parse(x[9]),
                MaxCombo = short.Parse(x[10]),
                
                Mods = (Mod) uint.Parse(x[13]),
                PlayMode = (PlayMode) byte.Parse(x[15]),
                Date = DateTime.UtcNow
            };
            
            score.Accuracy = Accuracy.GetAccuracy(score.Count300, score.Count100, score.Count50, score.CountMiss,
                                                  score.CountGeki, score.CountKatu, score.PlayMode);
            score.ScoreMd5 = Hex.ToHex(Crypto.GetMd5($"{score.Count300 + score.Count100}{score.FileMd5}{score.CountMiss}{score.CountGeki}{score.CountKatu}{score.Date}{score.Mods}"));
            score.ScoreOwner = Users.GetUser(factory, score.UserId);
            
            return (bool.Parse(x[14]), score);
        }
    }
}