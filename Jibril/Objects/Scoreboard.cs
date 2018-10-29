namespace Jibril.Objects
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Shared.Database.Models;
    using Shared.Enums;
    using Shared.Interfaces;

    public class Scoreboard : IOsuStringable
    {
        private List<Scores> _scores;
        
        private string _fileMD5;
        private Users _user;
        private PlayMode _playMode;
        private bool _relaxing;
        private bool _touching;
        private bool _friendsOnly;
        private bool _countryOnly;
        private bool _modOnly;
        private Mod _mods;
        private BeatmapSets _beatmapSets;

        public Scoreboard(string fileMd5, Users user,
                          PlayMode playMode = PlayMode.Osu, bool relaxing = false, bool touching = false,
                          bool friendsOnly = false, bool countryOnly = false, bool modOnly = false,
                          Mod mods = Mod.None)
        {
            _fileMD5 = fileMd5;
            _user = user;
            _playMode = playMode;
            _relaxing = relaxing;
            _touching = touching;
            _friendsOnly = friendsOnly;
            _countryOnly = countryOnly;
            _modOnly = modOnly;
            _mods = mods;
        }

        public string ToOsuString()
        {
            StringBuilder x = new StringBuilder();
            setScores();
            setBeatmap();
            
            x.Append(ScoreboardHeader());
            foreach (Scores score in _scores)
                x.Append($"{score.ToOsuString()}\n");

            return x.ToString();
        }

        private string ScoreboardHeader()
        {
            Beatmaps bm = _beatmapSets.GetBeatmap(_fileMD5);
            return $"{(int) bm.RankedStatus}|" +
                   $"{bm.Id}|" +
                   $"{bm.BeatmapSetId}|" +
                   $"{Scores.GetTotalScores(_fileMD5)}|" +
                   $"{0}|" +
                   $"{bm.Artist} - {bm.Title} [{bm.DifficultyName}]|" +
                   $"10\n";
        }
        
        private void setScores()
        {
            _scores = new List<Scores>
            {
                Scores.GetScores(_fileMD5, _user, _playMode, _relaxing, _touching, _friendsOnly, _countryOnly, _modOnly,
                    _mods, true).FirstOrDefault()
            };
            _scores.AddRange(Scores.GetScores(_fileMD5, _user, _playMode, _relaxing, _touching, _friendsOnly,
                _countryOnly, _modOnly, _mods));
        }

        private void setBeatmap()
        {
            
        }
    }
}
