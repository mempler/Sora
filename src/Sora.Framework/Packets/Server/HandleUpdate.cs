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

using Sora.Framework.Enums;
using Sora.Framework.Objects;
using Sora.Framework.Utilities;

namespace Sora.Framework.Packets.Server
{
    public class HandleUpdate : IPacket
    {
        public Presence Presence;

        public HandleUpdate(Presence presence) => Presence = presence;

        public PacketId Id => PacketId.ServerHandleOsuUpdate;

        public void ReadFromStream(MStreamReader sr)
        {
        }

        public void WriteToStream(MStreamWriter sw)
        {
            sw.Write(Presence.User.Id);
            sw.Write((byte) Presence.Status.Status);
            sw.Write(Presence.Status.StatusText, false);
            sw.Write(Presence.Status.BeatmapChecksum, false);
            sw.Write((uint) Presence.Status.CurrentMods);
            sw.Write((byte) Presence.Status.Playmode);
            sw.Write(Presence.Status.BeatmapId);

            sw.Write(Presence.Stats.RankedScore);
            sw.Write(Presence.Stats.Accuracy);
            sw.Write(Presence.Stats.PlayCount);
            sw.Write(Presence.Stats.TotalScore);
            sw.Write(Presence.Stats.Position);
            sw.Write(Presence.Stats.PerformancePoints);
        }
    }
}
