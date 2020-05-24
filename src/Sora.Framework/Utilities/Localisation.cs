using System;
using System.IO;
using MaxMind.GeoIP2;
using MaxMind.GeoIP2.Responses;
using Sora.Framework.Enums;

namespace Sora.Framework.Utilities
{
    public static class Localisation
    {
        public static void Initialize()
        {
            if (Directory.Exists("geoip") && Directory.Exists("geoip/GeoLite2-City") &&
                File.Exists("geoip/GeoLite2-City/GeoLite2-City.mmdb"))
                return;
            
            Logger.Fatal($"Couldn't find GeoLite2-City database at {Directory.GetCurrentDirectory()}/geoip/GeoLite2-City/GeoLite2-City.mmdb");
        }

        public static CityResponse GetData(string ip)
        {
            using (var client = new DatabaseReader("geoip/GeoLite2-City/GeoLite2-City.mmdb"))
                return client.City(ip);
        }

        public static CountryId StringToCountryId(string x) => Enum.TryParse(x, true, out CountryId o) ? o : CountryId.XX;


        // ReSharper disable once UnusedMember.Global
        public static string CountryIdToString(CountryId x) => x.ToString();
    }
}
