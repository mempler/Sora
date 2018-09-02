using System.IO;

using Kaoiji.enums;

namespace Kaoiji.packets
{
    public class Packet
    {
        public PacketIDs PacketID;
        public byte[] PacketData;
        public byte[] Write() => P2B(this);
        public static byte[] P2B(Packet p)
        {
            MemoryStream m = new MemoryStream();
            BinaryWriter w = new BinaryWriter(m);
            w.Write((short)p.PacketID);
            w.Write((byte)0);
            w.Write(p.PacketData == null ? 0 : p.PacketData.Length);
            w.Write(p.PacketData == null ? new byte[0] : p.PacketData);
            w.Close();
            return m.ToArray();
        }
        public static Packet B2P(byte[] b)
        {
            MemoryStream m = new MemoryStream(b);
            BinaryReader r = new BinaryReader(m);
            Packet p = new Packet();
            p.PacketID = (PacketIDs)r.ReadInt16();
            r.BaseStream.Position++;
            int DataLength = r.ReadInt32();
            p.PacketData = new byte[DataLength];
            r.Read(p.PacketData, (int)r.BaseStream.Position, DataLength);
            return p;
        }
    }
}
