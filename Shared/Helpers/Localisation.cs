#region copyright
/*
MIT License

Copyright (c) 2018 Robin A. P.

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
#endregion

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
    public static class Localisation
    {
        [Handler(HandlerTypes.Initializer)]
        public static void Initialize()
        {
            if (Directory.Exists("geoip") && Directory.Exists("geoip/GeoLite2-City") && File.Exists("geoip/GeoLite2-City/GeoLite2-City.mmdb")) return;

            WebRequest request = WebRequest.Create("http://geolite.maxmind.com/download/geoip/database/GeoLite2-City.tar.gz");
            using (WebResponse response = request.GetResponse())
            {
                Stream maxMindTarGz = response.GetResponseStream();
                Stream gzipStream = new GZipInputStream(maxMindTarGz);

                TarArchive tarArchive = TarArchive.CreateInputTarArchive(gzipStream);
                tarArchive.ExtractContents("geoip");
                tarArchive.Close();

                gzipStream.Close();
                maxMindTarGz?.Close();
            }

            foreach (string dir in Directory.GetDirectories("geoip"))
            {
                if (dir.Contains("GeoLite2-City"))
                {
                    Directory.Move(dir, "geoip/GeoLite2-City");
                }
            }
        }

        public static CityResponse GetData(string ip)
        {
            using (DatabaseReader client = new DatabaseReader("geoip/GeoLite2-City/GeoLite2-City.mmdb"))
                return client.City(ip);
        }

        public static CountryIds StringToCountryId(string x)
        {
            if (Enum.TryParse(typeof(CountryIds), x, true, out object o))
                return (CountryIds)o;
            return CountryIds.BL;
        }
        public static string CountryIdToString(CountryIds x) => x.ToString();
    }
}
