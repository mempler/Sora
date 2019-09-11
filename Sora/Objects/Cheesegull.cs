using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using Sora.Enums;
using Sora.Helpers;

namespace Sora.Objects
{
    public struct CheesegullBeatmapSet
    {
        public int SetID;
        public List<CheesegullBeatmap> ChildrenBeatmaps;
        public RankedStatus RankedStatus;
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
    }

    public struct CheesegullBeatmap
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

    public class Cheesegull
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

        private readonly ICheesegullConfig _cfg;
        private string _query;
        private List<CheesegullBeatmapSet> _sets;

        public Cheesegull(ICheesegullConfig cfg) => _cfg = cfg;

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

        public void Search(string query, int rankedStatus, int playMode, int page)
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
            var request_url = _cfg.Cheesegull.URI + "/api/search" + query;

            Logger.Debug($"{L_COL.YELLOW}Cheesegull {L_COL.WHITE}REQUEST_URI: {request_url}");

            var request = (HttpWebRequest)WebRequest.Create(request_url);
            request.AutomaticDecompression = DecompressionMethods.GZip;

            using (var response = (HttpWebResponse)request.GetResponse())
            using (var stream = response.GetResponseStream())
            using (var reader = new StreamReader(stream ?? throw new Exception("Request Failed!")))
            {
                var result = reader.ReadToEnd();
                _sets = JsonConvert.DeserializeObject<List<CheesegullBeatmapSet>>(result);
            }

            _query = query;
        }

        public void SetBMSet(int SetId)
        {
            var request_url = _cfg.Cheesegull.URI + "/api/s/" + SetId;

            var request = (HttpWebRequest)WebRequest.Create(request_url);
            request.AutomaticDecompression = DecompressionMethods.GZip;

            using var response = (HttpWebResponse)request.GetResponse();
            using var stream = response.GetResponseStream();
            using var reader = new StreamReader(stream ?? throw new Exception("Request Failed!"));
            var result = reader.ReadToEnd();
            _sets = new List<CheesegullBeatmapSet>(new[] { JsonConvert.DeserializeObject<CheesegullBeatmapSet>(result) });
        }

        public void SetBM(int BeatmapId)
        {
            var request_url = _cfg.Cheesegull.URI + "/api/b/" + BeatmapId;

            var request = (HttpWebRequest)WebRequest.Create(request_url);
            request.AutomaticDecompression = DecompressionMethods.GZip;

            using var response = (HttpWebResponse)request.GetResponse();
            using var stream = response.GetResponseStream();
            using var reader = new StreamReader(stream ?? throw new Exception("Request Failed!"));
            var result = reader.ReadToEnd();

            if (result == "null")
            {
                _sets = new List<CheesegullBeatmapSet>();
                return;
            }

            SetBMSet(JsonConvert.DeserializeObject<CheesegullBeatmap>(result).ParentSetID);
        }

        public void SetBM(string Hash)
        {
            var request_url = _cfg.Cheesegull.URI + "/api/hash/" + Hash;
            
            var request = (HttpWebRequest)WebRequest.Create(request_url);
            request.AutomaticDecompression = DecompressionMethods.GZip;

            using var response = (HttpWebResponse)request.GetResponse();
            using var stream = response.GetResponseStream();
            using var reader = new StreamReader(stream ?? throw new Exception("Request Failed!"));
            var result = reader.ReadToEnd();
            if (result != "null")
                _sets = new List<CheesegullBeatmapSet>(
                    new[] { JsonConvert.DeserializeObject<CheesegullBeatmapSet>(result) }
                );
            else
                _sets = new List<CheesegullBeatmapSet>();
        }

        public string ToDirect()
        {
            var RetStr = string.Empty;

            if (_sets == null)
                _sets = new List<CheesegullBeatmapSet>();

            RetStr += _sets.Count >= 100 ? "101" : _sets.Count.ToString();

            RetStr += "\n";

            if (_sets.Count > 0)
                foreach (var set in _sets)
                {
                    double MaxDiff = 0;

                    foreach (var cbm in set.ChildrenBeatmaps)
                        if (cbm.DifficultyRating > MaxDiff)
                            MaxDiff = cbm.DifficultyRating;

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

                    foreach (var cb in set.ChildrenBeatmaps)
                        RetStr += $"{cb.DiffName.Replace("@", "")} " +
                                  $"({Math.Round(cb.DifficultyRating, 2)}★~" +
                                  $"{cb.BPM}♫~AR" +
                                  $"{cb.AR}~OD" +
                                  $"{cb.OD}~CS" +
                                  $"{cb.CS}~HP" +
                                  $"{cb.HP}~" +
                                  $"{(int)MathF.Floor(cb.TotalLength) / 60}m" +
                                  $"{cb.TotalLength % 60}s)@" +
                                  $"{(int)cb.Mode},";

                    RetStr = RetStr.TrimEnd(',') + "|\r\n";
                }
            else if (_sets.Count <= 0)
                RetStr = "-1\nNo Beatmaps were found!";
            else if (_sets.Count <= 0 && _query == string.Empty)
                RetStr = "-1\nWhoops, looks like osu!direct is down!";

            return RetStr;
        }

        public string ToNP()
        {
            if (_sets.Count <= 0)
                return "0";

            var set = _sets[0];

            return $"{set.SetID}.osz|" +
                   $"{set.Artist}|" +
                   $"{set.Title}|" +
                   $"{set.Creator}|" +
                   $"{(int)set.RankedStatus}|" +
                   "10.00|" +
                   $"{set.LastUpdate}|" +
                   $"{set.SetID}|" +
                   $"{set.SetID}|" +
                   $"{Convert.ToInt32(set.HasVideo)}|" +
                   "0|" +
                   "1234|" +
                   $"{Convert.ToInt32(set.HasVideo) * 4321}\r\n";
        }

        public List<CheesegullBeatmapSet> GetSets() => _sets;
    }
}
