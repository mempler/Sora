using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Sora.Framework.Allocation;
using Sora.Framework.Enums;
using Sora.Framework.Objects.Multiplayer;
using Sora.Framework.Packets.Server;
using Sora.Framework.Utilities;

namespace Sora.Framework.Objects
{
    public class UserStatus
    {
        public Status Status;
        public string StatusText;
        public string BeatmapChecksum;
        public Mod CurrentMods;
        public PlayMode Playmode;
        public uint BeatmapId;

        public override string ToString()
            =>
                $"Status: {Status}, StatusText: {StatusText}, BeatmapChecksum: {CurrentMods}, Playmode: {Playmode}, BeatmapId: {BeatmapId}";
    }

    public class UserInformation
    {
        public byte TimeZone;
        public CountryId CountryId;
        public LoginPermissions ClientPermission;
        public double Longitude;
        public double Latitude;
    }

    public class UserStats
    {
        public ulong RankedScore;
        public float Accuracy;
        public uint PlayCount;
        public ulong TotalScore;
        public uint Position;
        public ushort PerformancePoints;
    }

    public class Token
    {
        internal string T { get; }

        public Token()
        {
            T = Guid.NewGuid().ToString();
        }

        public Token(string t)
        {
            T = t;
        }
        
        public override string ToString()
        {
            return T;
        }
    }

    public class TokenEqualityComparer : IEqualityComparer<Token>
    {
        public bool Equals(Token x, Token y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            return x.GetType() == y.GetType() && string.Equals(x.T, y.T);
        }

        public int GetHashCode(Token obj)
        {
            return (obj.T != null ? obj.T.GetHashCode() : 0) * 397;
        }
    }
    
    public class Presence : DynamicValues, IPacketPusher
    {
        public Token Token { get; set; }
        
        public User User { get; set;  }
        public UserStatus Status { get; set; }
        public UserInformation Info { get; set; }
        public UserStats Stats { get; set; }
        
        public Permission Permission => User.Permissions;
        
        public Match ActiveMatch { get; set; }
        public Spectator Spectator { get; set; }

        private List<IPacket> _packets = new List<IPacket>();
        private ReaderWriterLock _rwl = new ReaderWriterLock();

        public Presence(User user)
        {
            Token = new Token();
            User = user;
            Status = new UserStatus
            {
                Status = Enums.Status.Unknown,
                StatusText = "",
                BeatmapChecksum = ""
            };
            
            Info = new UserInformation
            {
                Latitude = 0,
                Longitude = 0,
                ClientPermission = LoginPermissions.User,
                CountryId = CountryId.XX,
                TimeZone = 0
            };
            
            Stats = new UserStats();
        }
        
        public void Push(IPacket packet, Presence skip = null)
        {
            try
            {
                _rwl.AcquireWriterLock(50);
                _packets.Add(packet);
            }
            finally
            {
                _rwl.ReleaseWriterLock();
            }
        }

        public void WritePackets(MStreamWriter writer)
        {
            if (this["IRC"] != null && (bool) this["IRC"])
                return;
            
            try
            {
                _rwl.AcquireWriterLock(5000);
                foreach (var packet in _packets)
                    writer.Write(packet);
                _packets.Clear();
            }
            finally
            {
                _rwl.ReleaseWriterLock();
            }
        }

        public void WritePackets(Stream stream)
            => WritePackets(new MStreamWriter(stream));

        public void Alert(string msg)
        {
            Push(new Announce(msg));
        }
    }
}