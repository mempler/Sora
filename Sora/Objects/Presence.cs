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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Shared.Enums;
using Shared.Helpers;
using Shared.Interfaces;
using Shared.Models;
using Sora.Enums;
using Sora.Packets.Client;
using Sora.Services;

namespace Sora.Objects
{
    public class Presence : IComparable
    {
        private readonly ChannelService _cs;
        //public bool Disconnected;

        // ReSharper disable once CollectionNeverUpdated.Global
        public readonly List<PacketStream> JoinedStreams = new List<PacketStream>();
        public readonly Stopwatch LastRequest;

        public bool BlockNonFriendDm;
        public int ClientPermissions;

        public CountryIds CountryId;

        //public DateTime BeginSeason;
        public bool IsLastRequest;
        public MultiplayerRoom JoinedRoom;
        public double Lat;
        public LeaderboardRx LeaderboardRx;

        //public UserStats Stats;
        public LeaderboardStd LeaderboardStd;
        public double Lon;

        public bool Relax;
        public SpectatorStream Spectator;

        public UserStatus
            Status = new UserStatus {BeatmapChecksum = "", StatusText = ""}; // Predefined strings to prevent Issues.

        public MStreamWriter Stream;
        public byte Timezone;
        public bool Touch;

        public Users User;

        public Presence(ChannelService cs)
        {
            _cs = cs;
            LastRequest = new Stopwatch();
            Token       = Guid.NewGuid().ToString();
            MemoryStream str = new MemoryStream();
            Stream = new MStreamWriter(str);
        }

        public string Token { get; }

        // ReSharper disable once MemberCanBeMadeStatic.Global
        public uint Rank => 0;

        public Channel PrivateChannel => _cs.GetChannel(User.Username);

        public int CompareTo(object obj)
        {
            if (obj.GetType() != typeof(Presence)) return -1;
            if (!(obj is Presence pr)) return -1;
            return pr.Token == Token ? 0 : 1;
        }

        protected bool Equals(Presence pr)
        {
            return Token == pr.Token;
        }

        public MemoryStream GetOutput(bool reset = true)
        {
            MemoryStream copy = new MemoryStream();
            long         pos  = Stream.BaseStream.Position;

            Stream.BaseStream.Position = 0;
            Stream.BaseStream.CopyTo(copy);
            Stream.BaseStream.Position = pos;
            LastRequest.Restart();

            if (!reset) return copy;

            using (Stream)
                // Dispose after chaning stream.
            {
                Stream = new MStreamWriter(new MemoryStream());
            }

            return copy;
        }

        public bool TimeoutCheck() => LastRequest.Elapsed.TotalSeconds > 30;

        public void Write(IPacket p)
        {
            if (!Stream.BaseStream.CanWrite) return;
            if (p != null) Stream?.Write(p);
        }
    }
}