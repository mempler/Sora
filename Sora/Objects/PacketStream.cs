using System.Collections.Generic;
using Shared.Enums;
using Shared.Handlers;
using Shared.Interfaces;

namespace Sora.Objects
{
    public class LPacketStreams
    {
        private static readonly Dictionary<string, PacketStream> PacketStreams = new Dictionary<string, PacketStream>();

        [Handler(HandlerTypes.Initializer)]
        public static void Initialize()
        {
            JoinStream("main", new PacketStream());
        }

        public static PacketStream GetStream(string name)
        {
            PacketStreams.TryGetValue(name, out var x);
            return x;
        }
        public static void JoinStream(string name, PacketStream str) =>
            PacketStreams[name] = str;

        public static void LeaveStream(string name) =>
            PacketStreams[name] = null;
    }

    public class PacketStream
    {
        public Dictionary<string, Presence> JoinedPresences = new Dictionary<string, Presence>();

        public void Join(Presence pr) => JoinedPresences[pr.Token] = pr;

        public void Left(Presence pr) => JoinedPresences[pr.Token] = null;
        public void Left(string token) => JoinedPresences[token] = null;

        public void Broadcast(IPacketSerializer packet, Presence ignorePresence = null)
        {
            foreach (var presence in JoinedPresences)
            {
                if (presence.Value == ignorePresence || presence.Value.Disconnected) continue;
                presence.Value.Write(packet);
            }
        }
    }
}
