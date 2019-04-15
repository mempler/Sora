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

using Shared.Enums;
using Shared.Helpers;
using Shared.Interfaces;
using Sora.Enums;

namespace Sora.Packets.Client
{
    public class SendUserStatus : IPacket
    {
        public UserStatus Status;
        public PacketId Id => PacketId.ClientSendUserStatus;

        public void ReadFromStream(MStreamReader sr)
        {
            Status = new UserStatus
            {
                Status          = (Status) sr.ReadByte(),
                StatusText      = sr.ReadString(),
                BeatmapChecksum = sr.ReadString(),
                CurrentMods     = (Mod) sr.ReadUInt32(),
                Playmode        = (PlayMode) sr.ReadByte(),
                BeatmapId       = sr.ReadUInt32()
            };
        }

        public void WriteToStream(MStreamWriter sw)
        {
            sw.Write((byte) Status.Status);
            sw.Write(Status.StatusText);
            sw.Write(Status.BeatmapChecksum);
            sw.Write((uint) Status.CurrentMods);
            sw.Write((byte) Status.Playmode);
            sw.Write(Status.BeatmapId);
        }
    }

    public struct UserStatus
    {
        public Status Status;
        public string StatusText;
        public string BeatmapChecksum;
        public Mod CurrentMods;
        public PlayMode Playmode;
        public uint BeatmapId;

        public override string ToString()
        {
            return
                $"Status: {Status}, StatusText: {StatusText}, BeatmapChecksum: {CurrentMods}, Playmode: {Playmode}, BeatmapId: {BeatmapId}";
        }
    }
}