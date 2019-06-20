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
using System.IO;
using System.Net;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;
using MaxMind.GeoIP2;
using MaxMind.GeoIP2.Responses;
using Sora.Enums;

namespace Sora.Helpers
{
    public static class Localisation
    {
        public static void Initialize()
        {
            if (Directory.Exists("geoip") && Directory.Exists("geoip/GeoLite2-City") &&
                File.Exists("geoip/GeoLite2-City/GeoLite2-City.mmdb"))
                return;

            var request =
                WebRequest.Create("http://geolite.maxmind.com/download/geoip/database/GeoLite2-City.tar.gz");
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
                if (dir.Contains("GeoLite2-City"))
                    Directory.Move(dir, "geoip/GeoLite2-City");
        }

        public static CityResponse GetData(string ip)
        {
            using var client = new DatabaseReader("geoip/GeoLite2-City/GeoLite2-City.mmdb");
            return client.City(ip);
        }

        public static CountryIds StringToCountryId(string x)
        {
            if (Enum.TryParse(typeof(CountryIds), x, true, out var o))
                return (CountryIds) o;
            return CountryIds.BL;
        }

        // ReSharper disable once UnusedMember.Global
        public static string CountryIdToString(CountryIds x) => x.ToString();
    }
}
