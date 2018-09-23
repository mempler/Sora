using System;
using System.IO;
using System.Net;
using MaxMind.GeoIP2;
using Shared.Enums;
using Shared.Handlers;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;
using MaxMind.GeoIP2.Responses;

namespace Shared.Helpers
{
    public class Localisation
    {
        [Handler(HandlerTypes.Initializer)]
        public static void Initialize()
        {
            if (Directory.Exists("geoip") && Directory.Exists("geoip/GeoLite2-City") && File.Exists("geoip/GeoLite2-City/GeoLite2-City.mmdb")) return;

            var request = WebRequest.Create("http://geolite.maxmind.com/download/geoip/database/GeoLite2-City.tar.gz");
            using (var response = request.GetResponse())
            {
                var maxMindTarGz = response.GetResponseStream();
                Stream gzipStream = new GZipInputStream(maxMindTarGz);

                var tarArchive = TarArchive.CreateInputTarArchive(gzipStream);
                tarArchive.ExtractContents("geoip");
                tarArchive.Close();

                gzipStream.Close();
                maxMindTarGz?.Close();
            }

            foreach (var dir in Directory.GetDirectories("geoip"))
            {
                if (dir.Contains("GeoLite2-City"))
                {
                    Directory.Move(dir, "geoip/GeoLite2-City");
                }
            }
        }

        public static CityResponse GetData(string ip)
        {
            using (var client = new DatabaseReader("geoip/GeoLite2-City/GeoLite2-City.mmdb"))
            {
                return client.City(ip);
            }
        }

        public static CountryIds StringToCountryId(string x)
        {
            if (Enum.TryParse(typeof(CountryIds), x, true, out var o))
                return (CountryIds)o;
            return CountryIds.BL;
        }
        public static string CountryIdToString(CountryIds x)
        {
            return x.ToString();
        }
    }
}
