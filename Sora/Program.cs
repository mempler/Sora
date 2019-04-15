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
using System.Diagnostics;
using System.Reflection;
using EventManager.Services;
using Microsoft.Extensions.DependencyInjection;
using Shared.Helpers;
using Shared.Models;
using Shared.Services;
using Sora.Helpers;
using Sora.Server;
using Sora.Services;

namespace Sora
{
    internal static class Program
    {
        private static IServiceProvider BuildProvider() =>
            new ServiceCollection()
                .AddSingleton<IConfig>(ConfigUtil.ReadConfig<Config>())
                .AddSingleton(ConfigUtil.ReadConfig<Config>())
                .AddSingleton<Database>()
                .AddSingleton<PluginService>()
                .AddSingleton<PresenceService>()
                .AddSingleton<MultiplayerService>()
                .AddSingleton<PacketStreamService>()
                .AddSingleton<ConsoleCommandService>()
                .AddSingleton<ChannelService>()
                .AddSingleton<StartupService>()
                .AddSingleton<HttpServer>()
                .AddSingleton<Bot.Sora>()
                .AddSingleton(new EventManager.EventManager(new List<Assembly> { Assembly.GetEntryAssembly() } ))

                .BuildServiceProvider();
        
        private static void Main()
        {
            Logger.Info(@"%#FFFFFF%Sora V1.0.0

%#800000%=============================== %#F94848%License %#800000%=================================
%#F94848%Sora - A Modular Bancho written in C#
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
%#800000%==========================================================================

");
            
            IServiceProvider provider = BuildProvider();
            
            Stopwatch w = new Stopwatch();
            Logger.Info("Generating %#F94848%Database%#FFFFFF%! this could take a while.");
            w.Start();
            // Create Sora (bot) if not exists.
            if (Users.GetUser(provider.GetService<Database>(), 100) == null)
                Users.InsertUser(provider.GetService<Database>(), new Users
                {
                    Id         = 100,
                    Username   = "Sora",
                    Email      = "bot@gigamons.de",
                    Password   = "",
                    Privileges = 0
                });
            w.Stop();
            Logger.Info("Done, it took%#3cfc59%", w.ElapsedMilliseconds + "ms");
            
            provider.GetService<StartupService>()
                    .Start()
                    .Wait();

            provider.GetService<ConsoleCommandService>()
                    .Start();

            provider.GetService<PresenceService>()
                    .TimeoutCheck();
   

            provider.GetService<HttpServer>()
                    .RunAsync()
                    .Wait();
        }
    }
}