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
using Shared.Enums;
using Shared.Handlers;
using Sora.Packets.Server;

namespace Sora.Objects
{
    public class Channels
    {
        // ReSharper disable once InconsistentNaming
        private static readonly Dictionary<string, Channel> Channels_ = new Dictionary<string, Channel>();
        public static readonly List<Channel> ChannelsAutoJoin = new List<Channel>();
        private static bool _initialized;

        [Handler(HandlerTypes.Initializer)]
        public static void Initialize()
        {
            if (_initialized) return;
            _initialized = true;
            LPacketStreams.Initialize();

            AddChannel(new Channel("#osu", "Osu! default channel.", LPacketStreams.GetStream("main"), autoJoin: true));
            AddChannel(new Channel("#announce", "Osu! default channel.", LPacketStreams.GetStream("main"), readOnly: true, autoJoin: true));
            AddChannel(new Channel("#userlog", "Osu! default channel.", LPacketStreams.GetStream("main"), readOnly: true));
            AddChannel(new Channel("#admin", "Admin. is an administration channel.", LPacketStreams.GetStream("admin"), adminOnly: true, autoJoin: true));
        }

        public static void AddChannel(Channel channel)
        {
            if (channel.AutoJoin)
                ChannelsAutoJoin.Add(channel);
            Channels_.TryAdd(channel.ChannelName, channel);
        }

        public static void RemoveChannel(Channel channel)
        {
            ChannelsAutoJoin.Remove(channel);
            Channels_.Remove(channel.ChannelName);
        }

        public static void RemoveChannel(string channelName)
        {
            var chan = GetChannel(channelName);
            if (chan == null) return;
            ChannelsAutoJoin.Remove(chan);
            Channels_.Remove(chan.ChannelName);
        }

        public static Channel GetChannel(string channelName)
        {
            Channels_.TryGetValue(channelName, out var x);
            return x;
        }
    }

    public class Channel
    {
        public Channel(string channelName, string channelTopic = "",
                       PacketStream boundStream = null, Presence channelOwner = null,
                       bool readOnly = false, bool adminOnly = false, bool autoJoin = false)
        {
            ChannelName = channelName; ChannelTopic = channelTopic; // Feels
            BoundStream = boundStream; ChannelOwner = channelOwner; // Good!
            ReadOnly = readOnly; AdminOnly = adminOnly; AutoJoin = autoJoin; // Feels bad!
        }

        public string ChannelName { get; }
        public string ChannelTopic { get; }
        public PacketStream BoundStream { get; }
        public Presence ChannelOwner { get; }
        public bool ReadOnly { get; set; }
        public bool AdminOnly { get; set; }
        public bool AutoJoin { get; set; }
        public int UserCount => _presences.Count;

        private readonly List<Presence> _presences = new List<Presence>(); // should be { get; } maybe ?

        public bool JoinChannel(Presence pr)
        {
            if (AdminOnly && (pr.User.Privileges & Privileges.Admin) != 0) { _presences.Add(pr); return true; }

            if (AdminOnly) { return false; }

            _presences.Add(pr);
            return true;
        }

        public bool LeaveChannel(Presence pr)
        {
            try
            {
                _presences.Remove(pr);
                return true;
            }
            catch { return false; }
        }

        public void WriteMessage(Presence pr, string message)
        {
            if (ReadOnly) return;
            BoundStream.Broadcast(
                new Message(
                    new MessageStruct {
                         Username = pr.User.Username,
                         ChannelTarget = ChannelName,
                         Message = message,
                         SenderId = pr.User.Id
                    }
                ),
            pr);
        }

        public override string ToString() => $"Channel: {ChannelName} ChannelTopic: {ChannelTopic} BoundStream: {BoundStream?.StreamName} ChannelOwner: {ChannelOwner?.User?.Username}";
    }
}
