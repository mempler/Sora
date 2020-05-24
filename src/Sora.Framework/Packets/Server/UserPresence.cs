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

using System;
using Sora.Framework.Enums;
using Sora.Framework.Objects;
using Sora.Framework.Utilities;

namespace Sora.Framework.Packets.Server
{
    public class UserPresence : IPacket
    {
        public readonly Presence Presence;

        public UserPresence(Presence pr) => Presence = pr;

        public PacketId Id => PacketId.ServerUserPresence;

        public void ReadFromStream(MStreamReader sr)
        {
            throw new NotImplementedException();
        }

        public void WriteToStream(MStreamWriter sw)
        {
            if (Presence == null)
                throw new ArgumentNullException();

            sw.Write( Presence.User.Id );
            sw.Write( Presence.User.UserName, false );
            sw.Write( (byte) (Presence.Info.TimeZone + 24) );
            sw.Write( (byte) Presence.Info.CountryId );
            sw.Write( (byte) Presence.Info.ClientPermission );
            sw.Write( (float) Presence.Info.Longitude );
            sw.Write( (float) Presence.Info.Latitude );
            sw.Write( Presence.Stats.Position );
        }
    }
}
