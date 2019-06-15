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
using Sora.Helpers;
using Sora.Objects;
using Fixer = Sora.Helpers.Fixer;
using PlayMode = Sora.Enums.PlayMode;
using RankedStatus = Sora.Enums.RankedStatus;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Sora.Database.Models
{
    public class Beatmaps
    {
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

        public static Beatmaps FetchFromApi(IConfig cfg, string fileMd5, int beatmapId = -1)
        {
            Cheesegull cg = new Cheesegull(cfg);
            if (!string.IsNullOrEmpty(fileMd5))
                cg.SetBM(fileMd5);
            else if (beatmapId != -1)
                cg.SetBM(beatmapId);

            List<CheesegullBeatmapSet> bmSet = cg.GetSets();
            if (bmSet.Count == 0)
                return null;

            CheesegullBeatmap bm = bmSet[0].ChildrenBeatmaps.FirstOrDefault(x => x.FileMD5 == fileMd5 || x.BeatmapID == beatmapId);
            if (string.IsNullOrEmpty(bm.FileMD5))
                return null;

            Beatmaps b = new Beatmaps
            {
                Ar               = bm.AR,
                Artist           = bmSet[0].Artist,
                Bpm              = bm.BPM,
                Cs               = bm.CS,
                Difficulty       = bm.DifficultyRating,
                Hp               = bm.HP,
                Id               = bm.BeatmapID,
                Od               = bm.OD,
                Tags             = bmSet[0].Tags,
                Title            = bmSet[0].Title,
                BeatmapCreator   = bmSet[0].Creator,
                BeatmapCreatorId = -1, // -1 because we do not own this beatmap.
                DifficultyName   = bm.DiffName,
                HitLength        = bm.HitLength,
                LastUpdate       = DateTime.Now,
                PassCount        = 0,
                PlayCount        = 0,
                PlayMode         = bm.Mode,
                RankedDate       = Convert.ToDateTime(bmSet[0].ApprovedDate),
                RankedStatus     = Fixer.FixRankedStatus((Cheesegull.RankedStatus) bmSet[0].RankedStatus),
                TotalLength      = bm.TotalLength,
                BeatmapSetId     = bm.ParentSetID,
                FileMd5          = bm.FileMD5
            };
            
            return b;
        }

        public static Beatmaps FetchFromDatabase(SoraDbContextFactory factory, string fileMd5, int beatmapId = -1) =>
            factory.Get().Beatmaps.FirstOrDefault(
                bm => bm.FileMd5 == fileMd5 ||
                      bm.Id ==beatmapId);

        public static void InsertBeatmap(SoraDbContextFactory factory, Beatmaps bm)
        {
            using DatabaseWriteUsage db = factory.GetForWrite();
            
            if (db.Context.BeatmapSets.Count(x => x.Id == bm.BeatmapSetId) < 1)
                db.Context.BeatmapSets.Add(new BeatmapSets
                {
                    Id             = bm.BeatmapSetId,
                    Beatmaps       = new List<Beatmaps>(new[] {bm}),
                    FavouriteCount = 0,
                    PassCount      = 0,
                    LastUpdate     = DateTime.UtcNow,
                    PlayCount      = 0
                });
            if (db.Context.Beatmaps.Count(x => x.Id == bm.Id) < 1)
                db.Context.Beatmaps.Add(bm);
            else
                db.Context.Beatmaps.Update(bm);
        }
    }
}