using System.Collections.Generic;
using System.Linq;
using Sora.Database;
using Sora.Database.Models;
using Sora.Objects;

namespace Sora
{
    public static class AchievementProcessor
    {
        
        /// <summary>
        /// Create Default Achievements
        /// </summary>
        /// <param name="factory">Context Factory</param>
        public static void CreateDefaultAchievements(SoraDbContextFactory factory)
        {
            if (factory.Get().Achievements.FirstOrDefault(x => x.Name == "oog") == null)
                Achievements.NewAchievement(
                    factory,
                    "oog",
                    "Oooooooooooooooog!",
                    "You just oooged JSE",
                    "https://onii-chan-please.come-inside.me/achivement_oog.png"
                );
        }

        /// <summary>
        /// Check if User has Obtained an achievement on this Score Submission
        /// </summary>
        /// <param name="factory">Context Factory</param>
        /// <param name="user">Who tries to Obtain</param>
        /// <param name="score">Submitted Score</param>
        /// <param name="map">Beatmap</param>
        /// <param name="set">Beatmap Set</param>
        /// <param name="oldLB">Old LeaderBoard</param>
        /// <param name="newLB">New LeaderBoard</param>
        /// <returns>Obtained Achievements</returns>
        public static string ProcessAchievements(SoraDbContextFactory factory,
            Users user,
            Scores score,
            CheesegullBeatmap map,
            CheesegullBeatmapSet set,
            LeaderboardStd oldLB,
            LeaderboardStd newLB)
        {
            var _l = new List<Achievements>();

            if ((int) newLB.PerformancePointsOsu == 4914)
            {
                var ach = Achievements.GetAchievement(factory, "oog");
                if (!user.AlreadyOptainedAchievement(ach))
                    _l.Add(ach);
            }

            // Insert custom achievements here. I'll implement a Plugin System later! but this will work for now.


            // END OF CUSTOM ACHIEVEMENTS

            var retVal = "";
            foreach (var ach in _l)
                retVal += ach.ToOsuString() + "/";
            retVal.TrimEnd('/');

            return retVal;
        }
    }
}
