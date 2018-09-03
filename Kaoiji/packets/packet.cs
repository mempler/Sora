using System.IO;

using Kaoiji.enums;
using Kaoiji.helpers;

namespace Kaoiji.packets
{
    public class PacketWriter
    {
        private BinaryWriter bw { get; }
        
        public void Announce(string msg)
        {
            Packet p = new Packet(PacketID.Server_Announce);
            Binary.WriteOsuString(msg, new BinaryWriter(p.Data));
            p.Write(bw);
        }
    }
    public class Packet
    {
        public Stream Data { get; }
        public PacketID ID;

        public Packet(PacketID ID) { this.ID = ID; Data = new MemoryStream(); }
        public Packet() { Data = new MemoryStream(); }

        public override string ToString() => $"PacketID: {ID}. PacketLength: {Data.Length}.";

        public void Write(BinaryWriter w)
        {
            w.Write((short)ID);
            w.Write((byte)0);
            w.Write(Data == null ? 0 : Data.Length);
            if (Data != null)
                Data.CopyTo(w.BaseStream);
            w.Close();
        }
        public Packet Read(BinaryReader r)
        {
            ID = (PacketID)r.ReadInt16();
            r.BaseStream.Position++;
            r.BaseStream.CopyTo(Data, r.ReadInt32());
            return this;
        }
    }
}
