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
using Newtonsoft.Json;

namespace Shared.Helpers
{
    public interface IConfig
    {
        MySql MySql { get; set; }
        Redis Redis { get; set; }
    }

    public static class ConfigUtil
    {
        public static T ReadConfig<T>() where T : IConfig, new()
        {
            if (File.Exists("config.json"))
                return JsonConvert.DeserializeObject<T>(File.ReadAllText("config.json"));
            T cfg = new T();

            File.WriteAllText("config.json", JsonConvert.SerializeObject(cfg, Formatting.Indented));
            Logger.Info("Config has been created! please edit.");
            Environment.Exit(0);
            return cfg;
        }
    }

    public struct Redis
    {
        public string Hostname;
    }

    public struct MySql
    {
        public string Username;
        public string Password;
        public string Hostname;
        public short Port;
        public string Database;
    }
}