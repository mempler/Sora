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
using Shared.Services;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Shared.Models
{
    public class BeatmapSets
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [NotMapped]
        public List<Beatmaps> Beatmaps { get; set; }

        [Required]
        public DateTime LastUpdate { get; set; }

        [Required]
        [DefaultValue(0)]
        public int FavouriteCount { get; set; }

        [Required]
        [DefaultValue(0)]
        public int PlayCount { get; set; }

        [Required]
        [DefaultValue(0)]
        public int PassCount { get; set; }

        public Beatmaps GetBeatmap(string fileMd5) => Beatmaps?.FirstOrDefault(b => b.FileMd5 == fileMd5);

        public static BeatmapSets GetBeatmapSet(Database db, int setId)
        {
            BeatmapSets sts = db.BeatmapSets.FirstOrDefault(s => s.Id == setId);

            if (sts != null)
                sts.Beatmaps = db.Beatmaps.Where(s => s.BeatmapSetId == sts.Id).ToList();

            return sts;
        }
    }
}