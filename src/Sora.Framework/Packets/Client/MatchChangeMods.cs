using System;
using Sora.Framework.Enums;
using Sora.Framework.Utilities;

namespace Sora.Framework.Packets.Client
{
    public class MatchChangeMods : IPacket
    {
        public Mod Mods;
        public PacketId Id => PacketId.ClientMatchChangeMods;

        public void ReadFromStream(MStreamReader sr)
        {
            Mods = (Mod) sr.ReadUInt32();
        }

        public void WriteToStream(MStreamWriter sw)
        {
            throw new NotImplementedException();
        }
    }
}
