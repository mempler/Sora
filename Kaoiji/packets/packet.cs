using System.IO;

using Kaoiji.enums;

namespace Kaoiji.packets
{
    public class Packet
    {
        private Stream PacketData;
        public PacketIDs PacketID;

        public void Write(BinaryWriter w) => WritePacket(this, w);

        public static void WritePacket(Packet p, BinaryWriter w)
        {
            w.Write((short)p.PacketID);
            w.Write((byte)0);
            w.Write(p.PacketData == null ? 0 : p.PacketData.Length);
            p.PacketData.CopyTo(w.BaseStream);
            w.Close();
        }

        public static Packet ReadPacket(BinaryReader r)
        {
            Packet p = new Packet();
            p.PacketID = (PacketIDs)r.ReadInt16();
            r.BaseStream.Position++;
            r.BaseStream.CopyTo(p.PacketData, r.ReadInt32());
            return p;
        }
    }
}
