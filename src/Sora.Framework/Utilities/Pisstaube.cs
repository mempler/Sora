using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sora.Framework.Enums;

namespace Sora.Framework.Utilities
{
    // ReSharper disable once UnassignedField.Global
    public class BeatmapSet
    {
        public int SetID;
        // ReSharper disable once CollectionNeverUpdated.Global
        public List<Beatmap> ChildrenBeatmaps;
        public Pisstaube.RankedStatus RankedStatus;
        public string ApprovedDate;
        public string LastUpdate;
        public string LastChecked;
        public string Artist;
        public string Title;
        public string Creator;
        public string Source;
        public string Tags;
        public bool HasVideo;
        public int Genre;
        public int Language;
        public int Favourites;
        
        public static string ToNP(BeatmapSet beatmapSet)
        {
            if (beatmapSet == null)
                return "0";

            return $"{beatmapSet.SetID}.osz|" +
                   $"{beatmapSet.Artist}|" +
                   $"{beatmapSet.Title}|" +
                   $"{beatmapSet.Creator}|" +
                   $"{(int) beatmapSet.RankedStatus}|" +
                   "10.00|" +
                   $"{beatmapSet.LastUpdate}|" +
                   $"{beatmapSet.SetID}|" +
                   $"{beatmapSet.SetID}|" +
                   $"{Convert.ToInt32(beatmapSet.HasVideo)}|" +
                   "0|" +
                   "1234|" +
                   $"{Convert.ToInt32(beatmapSet.HasVideo) * 4321}\r\n";
        }
    }

    public class Beatmap
    {
        public int BeatmapID;
        public int ParentSetID;
        public string DiffName;
        public string FileMD5;
        public PlayMode Mode;
        public float BPM;
        public float AR;
        public float OD;
        public float CS;
        public float HP;
        public int TotalLength;
        public int HitLength;
        public int Playcount;
        public int Passcount;
        public int MaxCombo;
        public double DifficultyRating;
    }

    public class SearchResult
    {
        public bool Failed;
        public List<BeatmapSet> Result;

        public string ToDirect()
        {
            var RetStr = string.Empty;

            if (Result == null)
                Result = new List<BeatmapSet>();

            RetStr += Result.Count >= 100 ? "101" : Result.Count.ToString();

            RetStr += "\n";

            if (Result.Count > 0)
                foreach (var set in Result)
                {
                    var MaxDiff = set.ChildrenBeatmaps.Select(cbm => cbm.DifficultyRating).Concat(new double[] {0}).Max();

                    MaxDiff *= 1.5;

                    RetStr += $"{set.SetID}.osz|" +
                              $"{set.Artist}|" +
                              $"{set.Title.Replace("|", "")}|" +
                              $"{set.Creator}|" +
                              $"{(int)set.RankedStatus}|" +
                              $"{MaxDiff:0.00}|" +
                              $"{set.LastUpdate}Z|" +
                              $"{set.SetID}|" +
                              $"{set.SetID}|" +
                              "0|" +
                              "1234|" +
                              $"{Convert.ToInt32(set.HasVideo)}|" +
                              $"{Convert.ToInt32(set.HasVideo) * 4321}|";

                    RetStr = set.ChildrenBeatmaps.Aggregate(RetStr,
                        (current, cb) =>
                            current + 
                             $"{cb.DiffName.Replace("@", "")} " +
                             $"({Math.Round(cb.DifficultyRating, 2)}★~" +
                             $"{cb.BPM}♫~AR" + $"{cb.AR}~OD" +
                             $"{cb.OD}~CS" + $"{cb.CS}~HP" +
                             $"{cb.HP}~" +
                             $"{(int) Math.Floor((double) cb.TotalLength) / 60}m" +
                             $"{cb.TotalLength % 60}s)@" +
                             $"{(int) cb.Mode},"
                    );

                    RetStr = RetStr.TrimEnd(',') + "|\r\n";
                }
            else if (Result.Count <= 0)
                RetStr = "-1\nNo Beatmaps were found!";
            else if (Failed)
                RetStr = "-1\nWhoops, looks like osu!direct is down!";

            return RetStr;
        }
    }
    
    public class Pisstaube
    {
        public enum RankedStatus
        {
            Graveyard = -2,
            Wip = -1,
            Pending = 0,
            Ranked = 1,
            Approved = 2,
            Qualified = 3,
            Loved = 4
        }
        
        private readonly IPisstaubeConfig _cfg;

        public Pisstaube(IPisstaubeConfig cfg)
        {
            _cfg = cfg;
        }

        private string GetFromEndpoint(string endPoint) => $"{_cfg.Pisstaube.URI}{endPoint}";

        #region Request Helpers

        private Task<byte[]> RequestBytesAsync(string endPoint)
        {
            using (var wc = new WebClient())
                return wc.DownloadDataTaskAsync(GetFromEndpoint(endPoint));
        }

        private async Task<string> RequestStringAsync(string endPoint)
            => Encoding.Default.GetString(await RequestBytesAsync(endPoint));

        private async Task<T> RequestJsonAsync<T>(string endPoint)
            => JsonConvert.DeserializeObject<T>(await RequestStringAsync(endPoint));

        #endregion
        
        #region Beatmap API Requests

        public Task<BeatmapSet> FetchBeatmapSetAsync(int beatmapSetId)
            => RequestJsonAsync<BeatmapSet>($"/api/cheesegull/s/{beatmapSetId}");

        public Task<BeatmapSet> FetchBeatmapSetAsync(string fileMd5)
            => RequestJsonAsync<BeatmapSet>($"/api/v1/hash/{fileMd5}");

        public Task<Beatmap> FetchBeatmapAsync(int beatmapId)
            => RequestJsonAsync<Beatmap>($"/api/cheesegull/b/{beatmapId}");

        #endregion
        
        #region Beatmap Search

        private static int CheeseStatus(int status)
        {
            switch (status)
            {
                case 0:
                    return 1;
                case 2:
                    return 0;
                case 3:
                    return 3;
                case 4:
                    return -100;
                case 5:
                    return -2;
                case 7:
                    return 2;
                case 8:
                    return 4;
                default:
                    return 1;
            }
        }

        public async Task<SearchResult> SearchAsync(string query, int rankedStatus, int playMode, int page)
        {
            query = query.ToLower();
            if (query.Contains("newest") || query.Contains("top rated") || query.Contains("most played"))
                query = "";

            rankedStatus = CheeseStatus(rankedStatus);

            string pm;
            if (playMode < 0 || playMode > 3)
                pm = "";
            else
                pm = $"mode={playMode.ToString()}&";

            query = $"?{pm}amount={100}&offset={page * 100}&status={rankedStatus}&query={query}";

            Logger.Debug($"{LCol.YELLOW}Pisstaube {LCol.WHITE}Query: {query}");
            SearchResult result;

            try
            {
                result = new SearchResult
                {
                    Failed = false,
                    Result = await RequestJsonAsync<List<BeatmapSet>>($"/api/cheesegull/search{query}")
                };
            }
            catch
            {
                result = new SearchResult
                {
                    Failed = true,
                    Result = new List<BeatmapSet>()
                };
            }

            return result;
        }
        
        #endregion

        public async Task<string> DownloadBeatmapAsync(string fileMd5)
        {
            if (!Directory.Exists("data/beatmaps"))
                Directory.CreateDirectory("data/beatmaps");

            if (File.Exists($"data/beatmaps/{fileMd5}"))
                return $"data/beatmaps/{fileMd5}";

            var bmSet = await FetchBeatmapSetAsync(fileMd5);
            var bm = bmSet.ChildrenBeatmaps.FirstOrDefault(cb => cb.FileMD5 == fileMd5);

            if (bm == null)
                return string.Empty;

            var bmBytes = await RequestBytesAsync($"/osu/{bm.FileMD5}");
            File.WriteAllBytes($"data/beatmaps/{bm.FileMD5}", bmBytes);

            return $"data/beatmaps/{bm.FileMD5}";
        }
    }
}