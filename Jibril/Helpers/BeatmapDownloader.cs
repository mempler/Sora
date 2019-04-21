using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Shared.Objects;

namespace Jibril.Helpers
{
    public static class BeatmapDownloader
    {
        public static string GetBeatmap(string hash, Config cfg)
        {
            if (!Directory.Exists("data/beatmaps"))
                Directory.CreateDirectory("data/beatmaps");

            if (File.Exists($"data/beatmaps/{hash}")) return $"data/beatmaps/{hash}";
            
            Cheesegull cg = new Cheesegull(cfg);
            cg.SetBM(hash);

            List<CheesegullBeatmapSet> sets = cg.GetSets();
            if (sets.Count < 1)
                return string.Empty;

            CheesegullBeatmap bm = sets[0].ChildrenBeatmaps.FirstOrDefault(cb => cb.FileMD5 == hash);
                
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create($"https://osu.ppy.sh/osu/{bm.BeatmapID}");
            request.AutomaticDecompression = DecompressionMethods.GZip;

            using (HttpWebResponse response = (HttpWebResponse) request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream ?? throw new Exception("Request Failed!")))
            {
                string result = reader.ReadToEnd();
                if (result.Length < 1) return string.Empty;
                File.WriteAllText($"data/beatmaps/{hash}", result);
            }

            return $"data/beatmaps/{hash}";
        }
    }
}