#region LICENSE

/*
    olSora - A Modular Bancho written in C#
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

using System.Collections.Generic;
using System.Text;
using Sora.Framework.Enums;
using Sora.Framework.Utilities;

namespace Sora.Framework.Objects.Scores
{
    public class Scoreboard
    {
        private Beatmap _bm;
        private readonly BeatmapSet _parent;
        private List<Score> _scores;
        private Score _ownScore;

        public Scoreboard(Beatmap bm, BeatmapSet bmParent, List<Score> scores, Score ownScore = null)
        {
            _bm = bm;
            _parent = bmParent;
            _scores = scores;
            _ownScore = ownScore;
        }
        
        public string ToOsuString()
        {
            var x = new StringBuilder();

            if (_bm == null)
            {
                Logger.Err("Failed to set Beatmap! Beatmap is null!");
                return $"{(int) RankedStatus.NeedUpdate}|false\n";
            }

            x.Append(ScoreboardHeader());
            if (_ownScore != null)
                x.Append($"{_ownScore?.ToOsuString()}\n");
            
            foreach (var score in _scores)
                x.Append($"{score?.ToOsuString()}\n");

            return x.ToString();
        }

        private string ScoreboardHeader()
        {
            if (_bm != null && _parent != null)
                return $"{(int) Fixer.FixRankedStatus(_parent.RankedStatus)}|false|" +
                       $"{_bm.BeatmapID}|" +
                       $"{_bm.ParentSetID}|" +
                       $"{_scores.Count}\n" +
                       "0\n" +
                       $"{_parent.Artist} - {_parent.Title} [{_bm.DiffName}]\n" +
                       "10.0\n";
            
            return $"{(int) RankedStatus.NeedUpdate}|false|" +
                   "-1|" +
                   "-1|" +
                   "-1\n" +
                   "0\n" +
                   "Unknown\n" +
                   "0\n";
        }
    }
}
