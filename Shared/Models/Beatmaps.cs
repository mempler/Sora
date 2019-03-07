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
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using opi.v1;
using Shared.Helpers;
using Shared.Services;
using PlayMode = Shared.Enums.PlayMode;
using RankedStatus = Shared.Enums.RankedStatus;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Shared.Models
{
    public class Beatmaps
    {
        private static Api _api;

        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [DefaultValue(RankedStatus.LatestPending)]
        public RankedStatus RankedStatus { get; set; }

        public DateTime RankedDate { get; set; }

        [Required]
        public DateTime LastUpdate { get; set; }

        [Required]
        [DefaultValue("Nobody")]
        public string Artist { get; set; }

        [Required]
        public int BeatmapSetId { get; set; }

        [Required]
        [DefaultValue(0)]
        public float Bpm { get; set; }

        [Required]
        [DefaultValue("Nobody")]
        public string BeatmapCreator { get; set; }

        [Required]
        [DefaultValue(0)]
        public int BeatmapCreatorId { get; set; }

        [Required]
        [DefaultValue(0d)]
        public double Difficulty { get; set; }

        [Required]
        [DefaultValue(0f)]
        public float Cs { get; set; }

        [Required]
        [DefaultValue(0f)]
        public float Od { get; set; }

        [Required]
        [DefaultValue(0f)]
        public float Ar { get; set; }

        [Required]
        [DefaultValue(0f)]
        public float Hp { get; set; }

        [Required]
        [DefaultValue(0f)]
        public int HitLength { get; set; }

        [Required]
        [DefaultValue("")]
        public string Title { get; set; }

        [Required]
        [DefaultValue(0)]
        public int TotalLength { get; set; }

        [Required]
        [DefaultValue("")]
        public string DifficultyName { get; set; }

        [Required]
        public string FileMd5 { get; set; }

        [Required]
        [DefaultValue(PlayMode.Osu)]
        public PlayMode PlayMode { get; set; }

        [Required]
        [DefaultValue("")]
        public string Tags { get; set; }

        [Required]
        [DefaultValue(0)]
        public int PlayCount { get; set; }

        [Required]
        [DefaultValue(0)]
        public int PassCount { get; set; }

        public static Beatmaps FetchFromApi(string fileMd5, int beatmapId = -1)
        {
            string osuApiKey = Config.ReadConfig().Osu.ApiKey;
            if (_api == null)
                _api = new Api(osuApiKey);

            Beatmap bm = _api.GetBeatmap(beatmapId, opi.v1.PlayMode.All, true, fileMd5);
            if (bm == null)
                return null;
            
            Beatmaps b = new Beatmaps
            {
                Ar               = bm.Ar,
                Artist           = bm.Artist,
                Bpm              = bm.Bpm,
                Cs               = bm.Cs,
                Difficulty       = bm.Difficulty,
                Hp               = bm.Hp,
                Id               = bm.BeatmapId,
                Od               = bm.Od,
                Tags             = bm.Tags,
                Title            = bm.Title,
                BeatmapCreator   = bm.Creator,
                BeatmapCreatorId = -1, // -1 because we do not own this beatmap.
                DifficultyName   = bm.DifficultyName,
                HitLength        = bm.HitLength,
                LastUpdate       = DateTime.Now,
                PassCount        = 0,
                PlayCount        = 0,
                PlayMode         = (PlayMode) bm.PlayMode,
                RankedDate       = bm.RankedDate ?? DateTime.Now,
                RankedStatus     = Fixer.FixRankedStatus(bm.RankedStatus),
                TotalLength      = bm.TotalLength,
                BeatmapSetId     = bm.BeatmapSetId,
                FileMd5          = bm.BeatmapMd5
            };

            return b;
        }

        public static Beatmaps FetchFromDatabase(Database db, string fileMd5, int beatmapId = -1) => 
            db.Beatmaps.FirstOrDefault(
                bm => bm.FileMd5 == fileMd5 ||
                      bm.Id ==beatmapId);

        public static void InsertBeatmap(Database db, Beatmaps bm)
        {
            if (db.BeatmapSets.Count(x => x.Id == bm.BeatmapSetId) < 1)
                db.BeatmapSets.Add(new BeatmapSets
                {
                    Id             = bm.BeatmapSetId,
                    Beatmaps       = new List<Beatmaps>(new[] {bm}),
                    FavouriteCount = 0,
                    PassCount      = 0,
                    LastUpdate     = DateTime.UtcNow,
                    PlayCount      = 0
                });
            if (db.Beatmaps.Count(x => x.Id == bm.Id) < 1)
                db.Beatmaps.Add(bm);
            else
                db.Beatmaps.Update(bm);

            db.SaveChanges();
        }
    }
}