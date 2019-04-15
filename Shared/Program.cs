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

// ReSharper disable All

// Only used for Migration. so just ignore it ^^

using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Org.BouncyCastle.Math.EC;
using Shared.Helpers;
using Shared.Services;

namespace Shared
{
    internal class Program
    {
        private static IServiceProvider BuildProvider() =>
            new ServiceCollection()
                .AddSingleton<Database>()
                .BuildServiceProvider();
        
        private static void Main()
        {
            IServiceProvider provider = BuildProvider();
            provider.GetService<Database>();
        }
    }
}