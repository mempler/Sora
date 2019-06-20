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
using Sora.Database;
using Sora.Database.Models;
using Sora.Enums;
using Sora.Helpers;

namespace Sora.Objects
{
    public class Scoreboard
    {
        private readonly Config _cfg;
        private readonly bool _countryOnly;

        private readonly SoraDbContextFactory _factory;
        private readonly string _fileMd5;
        private readonly bool _friendsOnly;
        private readonly bool _modOnly;
        private readonly Mod _mods;
        private readonly PlayMode _playMode;
        private readonly bool _relaxing;
        private readonly Users _user;
        private Beatmaps _bm;
        private List<Scores> _scores;

        public Scoreboard(SoraDbContextFactory factory, Config cfg,
            string fileMd5, Users user,
            PlayMode playMode = PlayMode.Osu, bool relaxing = false,
            bool friendsOnly = false, bool countryOnly = false, bool modOnly = false,
            Mod mods = Mod.None)
        {
            _factory = factory;
            _cfg = cfg;
            _fileMd5 = fileMd5;
            _user = user;
            _playMode = playMode;
            _relaxing = relaxing;
            _friendsOnly = friendsOnly;
            _countryOnly = countryOnly;
            _modOnly = modOnly;
            _mods = mods;
        }

        public string ToOsuString()
        {
            var x = new StringBuilder();
            SetScores();
            SetBeatmap();

            x.Append(ScoreboardHeader());
            foreach (var score in _scores)
                x.Append($"{score?.ToOsuString(_factory)}\n");

            return x.ToString();
        }

        private string ScoreboardHeader()
        {
            if (_bm != null)
                return $"{(int) _bm.RankedStatus}|false|" +
                       $"{_bm.Id}|" +
                       $"{_bm.BeatmapSetId}|" +
                       $"{Scores.GetTotalScores(_factory, _fileMd5)}\n" +
                       "0\n" +
                       $"{_bm.Artist} - {_bm.Title} [{_bm.DifficultyName}]\n" +
                       "10.0\n";
            return $"{(int) RankedStatus.NotSubmited}|false|" +
                   "-1|" +
                   "-1|" +
                   "-1\n" +
                   "0\n" +
                   "Unknown\n" +
                   "0\n";
        }

        private void SetScores()
        {
            _scores = new List<Scores>
            {
                Scores.GetScores(
                    _factory, _fileMd5, _user, _playMode, _relaxing, _friendsOnly, _countryOnly, _modOnly,
                    _mods, true
                ).FirstOrDefault()
            };
            _scores.AddRange(
                Scores.GetScores(
                    _factory, _fileMd5, _user, _playMode, _relaxing, _friendsOnly,
                    _countryOnly, _modOnly, _mods
                )
            );
        }

        private void SetBeatmap()
        {
            if ((_bm = Beatmaps.FetchFromDatabase(_factory, _fileMd5)) != null)
                return;
            if ((_bm = Beatmaps.FetchFromApi(_cfg, _fileMd5)) != null)
                Beatmaps.InsertBeatmap(_factory, _bm);
        }
    }
}
