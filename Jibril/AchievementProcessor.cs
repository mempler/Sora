using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Jibril.Objects;
using Shared.Models;
using Shared.Services;

namespace Jibril
{
    public static class AchievementProcessor
    {
        public static void CreateDefaultAchievements(Database db)
        {
            List<Achievements> alist = db.Achievements.ToList();
            
            if (alist.FirstOrDefault(x => x.Name != "oog") == null)
            Achievements.NewAchievement(db,
                                        "oog",
                                        "Oooooooooooooooog!",
                                        "You just oooged JSE",
                                        "https://onii-chan-please.come-inside.me/achivement_oog.png");
        }
        
        public static string ProcessAchievements(Database db,
                                                 
                                                 Users user,
                                                 Scores score,
                                                 CheesegullBeatmap map,
                                                 CheesegullBeatmapSet set,
                                                 
                                                 LeaderboardStd oldLB,
                                                 LeaderboardStd newLB)
        {
            List<Achievements> _l = new List<Achievements>();
            
            if ((int) newLB.PerformancePointsOsu == 4914) {
                Achievements ach = Achievements.GetAchievement(db, "oog");
                if (!user.AlreadyOptainedAchievement(ach))
                    _l.Add(ach);
            }

            // Insert custom achievements here. I'll implement a Plugin System later! but this will work for now.
            
            
            // END OF CUSTOM ACHIEVEMENTS
            
            string retVal = "";
            foreach (Achievements ach in _l) retVal += ach.ToOsuString(null) + "/";
            retVal.TrimEnd('/');
            
            return retVal;
        }
    }
}
