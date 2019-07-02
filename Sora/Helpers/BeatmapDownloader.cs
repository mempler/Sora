using System;
using System.IO;
using System.Linq;
using System.Net;
using Sora.Objects;

namespace Sora.Helpers
{
    public static class BeatmapDownloader
    {
        public static string GetBeatmap(string hash, ICheesegullConfig cfg, string beatmapPath = "data/beatmaps")
        {
            if (!Directory.Exists("data/beatmaps"))
                Directory.CreateDirectory("data/beatmaps");

            if (File.Exists($"{beatmapPath}/{hash}"))
                return $"{beatmapPath}/{hash}";

            var cg = new Cheesegull(cfg);
            cg.SetBM(hash);

            var sets = cg.GetSets();
            if (sets.Count < 1)
                return string.Empty;

            var bm = sets[0].ChildrenBeatmaps.FirstOrDefault(cb => cb.FileMD5 == hash);

            var request = (HttpWebRequest) WebRequest.Create($"https://osu.ppy.sh/osu/{bm.BeatmapID}");
            request.AutomaticDecompression = DecompressionMethods.GZip;

            using (var response = (HttpWebResponse) request.GetResponse())
            using (var stream = response.GetResponseStream())
            using (var reader = new StreamReader(stream ?? throw new Exception("Request Failed!")))
            {
                var result = reader.ReadToEnd();

                if (result.Length < 1)
                    return string.Empty;

                File.WriteAllText($"{beatmapPath}/{hash}", result);
            }

            return $"{beatmapPath}/{hash}";
        }
    }
}
