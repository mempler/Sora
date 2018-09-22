using System;
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
                CountryId = sr.ReadByte(),
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
            sw.Write(Presence.User.Username);
            sw.Write(Presence.Timezone);
            sw.Write(Presence.CountryId);
            sw.Write(Presence.ClientPermissions);
            sw.Write(Presence.Lat);
            sw.Write(Presence.Lon);
            sw.Write(Presence.Rank);
        }
    }
}
