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
using System.Threading;
using System.Threading.Tasks;
using Sora.Allocation;
using Sora.Database.Models;
using Sora.Enums;
using Sora.Helpers;
using Sora.Interfaces;
using Sora.Packets.Client;
using Sora.Packets.Server;
using Sora.Services;

namespace Sora.Objects
{
    public partial class Presence : DynamicValues, IComparable
    {
        private readonly ChannelService _cs;

        public readonly Stopwatch LastRequest;

        public readonly List<PacketStream> JoinedStreams = new List<PacketStream>();
        
        private readonly List<IPacket> packetList;

        public SpectatorStream Spectator;

        public Users User;

        public Presence(ChannelService cs)
        {
            _cs = cs;
            
            LastRequest = new Stopwatch();
            Token = Guid.NewGuid().ToString();
            packetList = new List<IPacket>();
            
            User = null;
        }

        public string Token { get; set; }

        public Channel PrivateChannel => _cs.GetChannel(User.Username);

        public int CompareTo(object obj)
        {
            if (obj.GetType() != typeof(Presence))
                return -1;
            if (!(obj is Presence pr))
                return -1;
            return pr.Token == Token ? 0 : 1;
        }

        public MemoryStream GetOutput()
        {
            LastRequest.Restart();
            IPacket[] copy;

            Monitor.Enter(packetList);
            try
            {
                copy = new IPacket[packetList.Count];
                packetList.CopyTo(copy);
                packetList.Clear();
            }
            finally
            {
                Monitor.Exit(packetList);
            }
            
            var res = MStreamWriter.New();
            
            res.Write(copy);

            res.BaseStream.Position = 0;

            return (MemoryStream) res.BaseStream;
        }
        
        public bool TimeoutCheck() => LastRequest.Elapsed.Seconds > 60;
        
        public Presence Write(IPacket p)
        {
            if (this["IRC"] != null && (bool) this["IRC"])
                return this;
            
            if (p == null)
                return this;

            lock (packetList)
            {
                packetList.Add(p);
            }

            return this;
        }
        public static Presence operator +(Presence instance, IPacket p) => instance.Write(p);
    }
}
