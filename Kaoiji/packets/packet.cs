using System.IO;

using Kaoiji.enums;
using Kaoiji.helpers;

namespace Kaoiji.packets
{
    /// <summary>
    /// PacketWriter is the easy way to write packets.
    /// </summary>
    public class PacketWriter
    {
        private BinaryWriter Buffer { get; }
        
        /// <summary>
        /// Writes an Announcement (Yellow)
        /// </summary>
        public void Announce(string msg, params string[] args)
        {
            Packet p = new Packet(PacketID.Server_Announce);
            Binary.WriteOsuString(string.Format(msg, args), new BinaryWriter(p.Data));
            p.Write(Buffer);
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
