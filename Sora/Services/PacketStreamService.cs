using System.Collections.Generic;
using Sora.Objects;

namespace Sora.Services
{
    public class PacketStreamService
    {
        private readonly Dictionary<string, PacketStream> _packetStreams = new Dictionary<string, PacketStream>();
        
        public PacketStreamService()
        {
            NewStream(new PacketStream("main"));
            NewStream(new PacketStream("admin"));
            NewStream(new PacketStream("lobby"));
        }

        public PacketStream GetStream(string name)
        {
            _packetStreams.TryGetValue(name, out PacketStream x);
            return x;
        }

        public void NewStream(PacketStream str) => _packetStreams[str.StreamName] = str;

        public void RemoveStream(string name) => _packetStreams[name] = null;
    }
}