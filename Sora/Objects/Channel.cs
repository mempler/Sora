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
using System.Threading;
using Shared.Enums;
using Shared.Handlers;
using Sora.Packets.Server;

namespace Sora.Objects
{
    public static class LChannels
    {
        public static readonly Dictionary<string, Channel> Channels = new Dictionary<string, Channel>();
        public static readonly List<Channel> ChannelsAutoJoin = new List<Channel>();
        private static bool _initialized;

        [Handler(HandlerTypes.Initializer)]
        public static void Initialize()
        {
            if (_initialized) return;
            _initialized = true;
            LPacketStreams.Initialize();

            AddChannel(new Channel("#osu", "Osu! default channel.", LPacketStreams.GetStream("main"), autoJoin: true));
            AddChannel(new Channel("#announce", "Osu! default channel.", LPacketStreams.GetStream("main"),
                                   readOnly: true,
                                   autoJoin: true));
            AddChannel(new Channel("#userlog", "Osu! default channel.", LPacketStreams.GetStream("main"),
                                   readOnly: true));
            AddChannel(new Channel("#admin", "Admin. is an administration channel.", LPacketStreams.GetStream("admin"),
                                   adminOnly: true, autoJoin: true));
        }

        public static void AddChannel(Channel channel)
        {
            if (channel.AutoJoin)
                ChannelsAutoJoin.Add(channel);
            Channels.TryAdd(channel.ChannelName, channel);
        }

        public static void RemoveChannel(Channel channel)
        {
            ChannelsAutoJoin.Remove(channel);
            Channels.Remove(channel.ChannelName);
        }

        public static void RemoveChannel(string channelName)
        {
            Channel chan = GetChannel(channelName);
            if (chan == null) return;
            ChannelsAutoJoin.Remove(chan);
            Channels.Remove(chan.ChannelName);
        }

        public static Channel GetChannel(string channelName)
        {
            Channels.TryGetValue(channelName, out Channel x);
            return x;
        }
    }

    public class Channel
    {
        private readonly List<Presence> _presences = new List<Presence>(); // should be { get; } maybe ?

        private readonly Mutex _mut = new Mutex();

        private int _userCount = -1;

        public Channel(string channelName, string channelTopic = "",
                       PacketStream boundStream = null, Presence boundPresence = null,
                       bool readOnly = false, bool adminOnly = false, bool autoJoin = false)
        {
            ChannelName   = channelName;
            ChannelTopic  = channelTopic;
            BoundStream   = boundStream;
            BoundPresence = boundPresence;
            ReadOnly      = readOnly;
            AdminOnly     = adminOnly;
            AutoJoin      = autoJoin;
        }

        public string ChannelName { get; }
        public string ChannelTopic { get; }
        public PacketStream BoundStream { get; }
        public Presence BoundPresence { get; }
        public bool ReadOnly { get; set; }
        public bool AdminOnly { get; }
        public bool AutoJoin { get; }

        public int UserCount
        {
            get
            {
                if (_userCount > -1) return _userCount;
                if (_presences == null) return 0;

                _mut.WaitOne();
                int c = _presences.Count;
                _mut.ReleaseMutex();

                return c;
            }
            set => _userCount = value;
        }

        public bool JoinChannel(Presence pr)
        {
            if (AdminOnly && (pr.User.Privileges & Privileges.Admin) != 0)
            {
                _mut.WaitOne();
                _presences.Remove(pr);
                _presences.Add(pr);
                _mut.ReleaseMutex();

                return true;
            }

            if (AdminOnly) return false;

            _mut.WaitOne();
            _presences.Remove(pr);
            _presences.Add(pr);
            _mut.ReleaseMutex();

            return true;
        }

        public void LeaveChannel(Presence pr)
        {
            try
            {
                lock (_presences) { _presences.Remove(pr); }
            }
            catch
            {
                // Ignored
            }
        }

        public void WriteMessage(Presence pr, string message)
        {
            if (ReadOnly) return;
            if (BoundStream == null && BoundPresence != null)
            {
                BoundPresence.Write(
                    new SendIrcMessage(
                        new MessageStruct
                        {
                            Username      = pr.User.Username,
                            ChannelTarget = pr.User.Username,
                            Message       = message,
                            SenderId      = pr.User.Id
                        })
                );
                return;
            }
            
            BoundStream?.Broadcast(
                new SendIrcMessage(
                    new MessageStruct
                    {
                        Username      = pr.User.Username,
                        ChannelTarget = ChannelName,
                        Message       = message,
                        SenderId      = pr.User.Id
                    }
                ), pr);
        }

        public override string ToString()
        {
            return
                $"Channel: {ChannelName} ChannelTopic: {ChannelTopic} BoundStream: {BoundStream?.StreamName} ChannelOwner: {BoundPresence?.User?.Username}";
        }
    }
}