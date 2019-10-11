#region LICENSE

/*
    olSora - A Modular Bancho written in C#
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

using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;

namespace Sora
{
    public interface IPlugin
    {
        /// <summary>
        /// Enable Plugin
        /// </summary>
        void OnEnable(IApplicationBuilder app);
        
        /// <summary>
        /// Disable Plugin
        /// </summary>
        void OnDisable();
    }

    [MeansImplicitUse]
    public class Plugin : IPlugin
    {
        public virtual void OnEnable(IApplicationBuilder app)
        {
        }

        public virtual void OnDisable()
        {
        }
    }
}
