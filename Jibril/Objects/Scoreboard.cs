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

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jibril.Helpers;
using Shared.Enums;
using Shared.Interfaces;
using Shared.Models;
using Shared.Services;

namespace Jibril.Objects
{
    public class Scoreboard : IOsuStringable
    {
        private readonly bool _countryOnly;

        private readonly Database _db;
        private readonly Config _cfg;
        private readonly string _fileMd5;
        private readonly bool _friendsOnly;
        private readonly bool _modOnly;
        private readonly Mod _mods;
        private readonly PlayMode _playMode;
        private readonly bool _relaxing;
        private readonly bool _touching;
        private readonly Users _user;
        private Beatmaps _bm;
        private List<Scores> _scores;

        public Scoreboard(Database db, Config cfg,
                          string fileMd5, Users user,
                          PlayMode playMode = PlayMode.Osu, bool relaxing = false, bool touching = false,
                          bool friendsOnly = false, bool countryOnly = false, bool modOnly = false,
                          Mod mods = Mod.None)
        {
            _db = db;
            _cfg = cfg;
            _fileMd5     = fileMd5;
            _user        = user;
            _playMode    = playMode;
            _relaxing    = relaxing;
            _touching    = touching;
            _friendsOnly = friendsOnly;
            _countryOnly = countryOnly;
            _modOnly     = modOnly;
            _mods        = mods;
        }

        public string ToOsuString(Database db)
        {
            StringBuilder x = new StringBuilder();
            SetScores();
            SetBeatmap();

            x.Append(ScoreboardHeader());
            foreach (Scores score in _scores)
                x.Append($"{score?.ToOsuString(db)}\n");

            return x.ToString();
        }

        private string ScoreboardHeader()
        {
            if (_bm != null)
                return $"{(int) _bm.RankedStatus}|false|" +
                       $"{_bm.Id}|" +
                       $"{_bm.BeatmapSetId}|" +
                       $"{Scores.GetTotalScores(_db, _fileMd5)}\n" +
                       $"{0}\n" +
                       $"{_bm.Artist} - {_bm.Title} [{_bm.DifficultyName}]\n" +
                       "10.0\n";
            return $"{(int) RankedStatus.NotSubmited}|false|" +
                   $"-1|" +
                   $"-1|" +
                   $"-1\n" +
                   $"0\n" +
                   $"Unknown\n" +
                   "0\n";
        }

        private void SetScores()
        {
            _scores = new List<Scores>
            {
                Scores.GetScores(_db, _fileMd5, _user, _playMode, _relaxing, _touching, _friendsOnly, _countryOnly, _modOnly,
                                 _mods, true).FirstOrDefault()
            };
            _scores.AddRange(Scores.GetScores(_db, _fileMd5, _user, _playMode, _relaxing, _touching, _friendsOnly,
                                              _countryOnly, _modOnly, _mods));
        }

        private void SetBeatmap()
        {
            if ((_bm = Beatmaps.FetchFromDatabase(_db, _fileMd5)) != null) return;
            if ((_bm = Beatmaps.FetchFromApi(_cfg.Osu.OsuAPIKey, _fileMd5)) != null)
                Beatmaps.InsertBeatmap(_db, _bm);
        }
    }
}