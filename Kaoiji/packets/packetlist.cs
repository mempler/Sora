using System.Linq;
using System.IO;
using System;
using System.Collections.Generic;

namespace Kaoiji.packets
{
    public class PacketList
    {
        private List<Packet> Packets = new List<Packet>();
        public PacketList(PacketList l) => Packets = l.Packets;
        public PacketList(Packet[] ps) { foreach (Packet p in ps) Packets.Add(p); }
        public PacketList(Packet p) => Packets.Append(p);
        public PacketList() { }

        public void Append(Packet p) => Packets.Add(p);
        
        public byte[] ToBinary()
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms);
            foreach (Packet p in Packets)
                bw.Write(p.Write());
            bw.Close();
            return ms.ToArray();
        }
    }
}
