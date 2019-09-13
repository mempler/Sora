using System;
using Sora.Enums;
using Sora.Framework.Allocation;
using Sora.Framework.Enums;
using Sora.Framework.Objects.Multiplayer;
using Sora.Framework.Utilities;

namespace Sora.Framework.Objects
{
    public struct UserStatus
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

    public struct UserInformation
    {
        public byte TimeZone;
        public CountryId CountryId;
        public LoginPermissions ClientPermission;
        public double Longitude;
        public double Latitude;
        public int RankingPosition;
    }
    
    public class Presence : DynamicValues, IPacketPusher
    {
        public string Token { get; }
        
        public User User { get; }
        public UserStatus Status { get; set; }
        public UserInformation Info { get; set; }
        
        public Permission Permission => User.Permissions;
        
        public Match ActiveMatch { get; set; }

        public Presence(User user)
        {
            Token = Guid.NewGuid().ToString();
            User = user;
        }
        
        public void Push(IPacket packet)
        {
            throw new System.NotImplementedException();
        }
    }
}