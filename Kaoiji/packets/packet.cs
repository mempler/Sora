using System.IO;

using Kaoiji.enums;
using Kaoiji.helpers;

namespace Kaoiji.packets
{
    public class PacketWriter
    {
        private BinaryWriter bw;
        public void Announce(string msg)
        {
            using (MemoryStream s = new MemoryStream())
            using (BinaryWriter bw = new BinaryWriter(s))
            {
                Packet p = new Packet(PacketIDs.Server_Announce, s);
                Binary.WriteOsuString(msg, bw);
                Packet.WritePacket(p, bw);
            }
        }
    }
    public class Packet
    {
        public Stream Data { get; }
        public PacketIDs ID;
        public Packet(PacketIDs ID, Stream Data) { this.ID = ID; this.Data = Data; }
        public Packet() { }

        public static void WritePacket(Packet p, BinaryWriter w)
        {
            w.Write((short)p.ID);
            w.Write((byte)0);
            w.Write(p.Data == null ? 0 : p.Data.Length);
            p.Data.CopyTo(w.BaseStream);
            w.Close();
        }
        public static Packet ReadPacket(BinaryReader r)
        {
            Packet p = new Packet();
            p.ID = (PacketIDs)r.ReadInt16();
            r.BaseStream.Position++;
            r.BaseStream.CopyTo(p.Data, r.ReadInt32());
            return p;
        }
    }
}
