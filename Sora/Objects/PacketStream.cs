#region copyright
/*
MIT License

Copyright (c) 2018 Robin A. P.

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
#endregion

using System.Collections.Generic;
using System.Linq;
using Shared.Enums;
using Shared.Handlers;
using Shared.Helpers;
using Shared.Interfaces;

namespace Sora.Objects
{
    public static class LPacketStreams
    {
        private static readonly Dictionary<string, PacketStream> PacketStreams = new Dictionary<string, PacketStream>();
        private static bool _initialized;

        [Handler(HandlerTypes.Initializer)]
        public static void Initialize()
        {
            if (_initialized) return;
            _initialized = true;

            NewStream(new PacketStream("main"));
            NewStream(new PacketStream("admin")); // Admin stream, only admins will join.
            NewStream(new PacketStream("lobby"));
        }

        public static PacketStream GetStream(string name)
        {
            PacketStreams.TryGetValue(name, out PacketStream x);
            return x;
        }

        public static void NewStream(PacketStream str) { PacketStreams[str.StreamName] = str; }

        public static void RemoveStream(string name) { PacketStreams[name] = null; }
    }

    public class PacketStream
    {
        private readonly Dictionary<string, Presence> _joinedPresences = new Dictionary<string, Presence>();

        public PacketStream(string name) { StreamName = name; }

        public string StreamName { get; }

        public int JoinedUsers => _joinedPresences.Count;

        public void Join(Presence pr) { _joinedPresences.Add(pr.Token, pr); }

        public void Left(Presence pr) { _joinedPresences.Remove(pr.Token); }

        public void Left(string token) { _joinedPresences.Remove(token); }

        public void Broadcast(IPacket packet, params Presence[] ignorePresences)
        {
            foreach (KeyValuePair<string, Presence> presence in _joinedPresences)
            {
                if (presence.Value == null)
                {
                    Left(presence.Key);
                    continue;
                }
                if (ignorePresences.Contains(presence.Value))
                    continue;
                
                if (packet == null)
                    Logger.L.Error("PACKET IS NULL!");
                
                presence.Value?.Write(packet);
            }
        }
    }
}