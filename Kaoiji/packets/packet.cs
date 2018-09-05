using System;
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
        public BinaryWriter Buffer { get; }
        public PacketWriter(Stream s) => Buffer = new BinaryWriter(s);
        public PacketWriter(BinaryWriter bw) => Buffer = bw;
        public PacketWriter(PacketWriter pw) => Buffer = pw.Buffer;
        public PacketWriter() => Buffer = new BinaryWriter(new MemoryStream());

        #region Actual Packets
        /// <summary>
        /// Writes an Announcement (Yellow)
        /// </summary>
        public void Announce(string msg, params string[] args)
        {
            Packet p = new Packet(PacketID.Server_Announce);
            Binary.WriteOsuString(string.Format(msg, args), p.Data);
            p.Write(Buffer);
        }

        /// <summary>
        /// Writes an Protocol negotation.
        /// </summary>
        public void Protocol_Negotiation()
        {
            Packet p = new Packet(PacketID.Server_ProtocolNegotiation);
            p.Data.Write(Program.ProtocolVersion);
            p.Write(Buffer);
        }

        /// <summary>
        /// Writes an LoginResponse
        /// </summary>
        /// <param name="response">Response is a UserID or an Enum</param>
        public void LoginResponse<T>(object response) where T : IConvertible
        {
            if (typeof(T) != typeof(LoginResponses) || typeof(T) != typeof(int))
                return;
            Packet p = new Packet(PacketID.Server_LoginResponse);
            p.Data.Write((int)response);
            p.Write(Buffer);
        }
        #endregion

        public void WritePackets(Stream s) {
            Buffer.BaseStream.CopyTo(s);
            Buffer.Flush();
        }

        public byte[] WritePackets() {
            MemoryStream ms = new MemoryStream();
            Buffer.BaseStream.CopyTo(ms);
            Buffer.Flush();
            return ms.ToArray();
        }
    }
    public class Packet
    {
        public BinaryWriter Data { get; }
        public PacketID ID;

        public Packet(PacketID ID) { this.ID = ID; Data = new BinaryWriter(new MemoryStream()); }
        public Packet() { Data = new BinaryWriter(new MemoryStream()); }

        public override string ToString() => $"PacketID: {ID}. PacketLength: {Data.BaseStream.Length}.";

        public void Write(BinaryWriter w)
        {
            w.Write((short)ID);
            w.Write((byte)0);
            w.Write(Data == null ? 0 : Data.BaseStream.Length);
            if (Data != null)
                Data.BaseStream.CopyTo(w.BaseStream);
        }
        public Packet Read(BinaryReader r)
        {
            ID = (PacketID)r.ReadInt16();
            r.BaseStream.Position++;
            r.BaseStream.CopyTo(Data.BaseStream, r.ReadInt32());
            return this;
        }
    }
}
