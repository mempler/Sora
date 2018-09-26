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

using System.Data;
using Shared.Database.Models;
using Shared.Enums;
using Shared.Helpers;
using Shared.Interfaces;
using Sora.Objects;

namespace Sora.Packets.Server
{
    public class UserPresence : IPacketSerializer
    {
        public PacketId Id => PacketId.ServerUserPresence;

        public Presence Presence;

        public UserPresence() { }

        public UserPresence(Presence pr) { Presence = pr; }

        public void ReadFromStream(MStreamReader sr)
        {
            Presence = new Presence
            {
                User = new Users
                {
                    Id = sr.ReadInt32(),
                    Username = sr.ReadString(),
                },
                Timezone = sr.ReadByte(),
                CountryId = (CountryIds)sr.ReadByte(),
                ClientPermissions = sr.ReadByte(),
                Lon = sr.ReadDouble(),
                Lat = sr.ReadDouble(),
                Rank = sr.ReadUInt32()
            };
        }

        public void WriteToStream(MStreamWriter sw)
        {
            if (Presence == null)
                throw new NoNullAllowedException("Presence cannot be null!");

            sw.Write(Presence.User.Id);
            sw.Write(Presence.User.Username, false);
            sw.Write(Presence.Timezone);
            sw.Write((byte)Presence.CountryId);
            sw.Write(Presence.ClientPermissions);
            sw.Write(Presence.Lat);
            sw.Write(Presence.Lon);
            sw.Write(Presence.Rank);
        }
    }
}
