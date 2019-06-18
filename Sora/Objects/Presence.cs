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
using Sora.Enums;
using Sora.Packets.Client;
using Sora.Services;
using CountryIds = Sora.Enums.CountryIds;
using IPacket = Sora.Interfaces.IPacket;
using LeaderboardRx = Sora.Database.Models.LeaderboardRx;
using LeaderboardStd = Sora.Database.Models.LeaderboardStd;
using MStreamWriter = Sora.Helpers.MStreamWriter;
using Users = Sora.Database.Models.Users;

#nullable enable
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
        public LoginPermissions ClientPermissions;

        public CountryIds CountryId;

        //public DateTime BeginSeason;
        public bool IsLastRequest;
        public MultiplayerRoom? JoinedRoom;
        public double Lat;
        public LeaderboardRx LeaderboardRx;

        //public UserStats Stats;
        public LeaderboardStd LeaderboardStd;
        public double Lon;

        public bool Relax;
        public SpectatorStream? Spectator;

        public UserStatus
            Status = new UserStatus {BeatmapChecksum = "", StatusText = ""}; // Predefined strings to prevent Issues.

        public object locker;
        //public MStreamWriter Stream;
        public List<IPacket> packetList;
        public byte Timezone;

        public Users User;

        public bool BotPresence;

        public Presence(ChannelService cs)
        {
            _cs = cs;

            locker      = new object();
            LastRequest = new Stopwatch();
            Token       = Guid.NewGuid().ToString();
            //Stream      = new MStreamWriter(new MemoryStream());
            packetList = new List<IPacket>();
            LeaderboardRx = null;
            LeaderboardStd = null;
            User = null;
        }

        public string Token { get; }

        // ReSharper disable once MemberCanBeMadeStatic.Global
        public uint Rank = 0;

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

        public MemoryStream GetOutput()
        {
            LastRequest.Restart();

            IPacket[] copy;
            lock (locker)
            {
                copy = new IPacket[packetList.Count];
                packetList.CopyTo(copy);
                packetList.Clear();
            }
            
            MStreamWriter res = MStreamWriter.New();

            foreach (IPacket p in copy)
                res.Write(p);
            
            res.BaseStream.Position = 0;
            
            return (MemoryStream) res.BaseStream;
        }

        public bool TimeoutCheck() => LastRequest.Elapsed.Seconds > 60;

        public void Write(IPacket p)
        {
            if (BotPresence) return;
            if (p == null) return;
            
            lock(packetList)
                packetList.Add(p);
        }
    }
}