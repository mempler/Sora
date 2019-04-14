#region LICENSE
/*
    Sora - A Modular Bancho written in C#
    Copyright (C) 2019 Robin A. P.

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Affero General Public License as
    published by the Free Software Foundation, either version 3 of the
    License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Affero General Public License for more details.

    You should have received a copy of the GNU Affero General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
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

        public Users User;

        public bool BotPresence;

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
            if (BotPresence) return;
            if (!Stream.BaseStream.CanWrite) return;
            if (p != null) Stream?.Write(p);
        }
    }
}