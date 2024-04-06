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
