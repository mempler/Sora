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
using System.Text;
using Shared.Enums;
using Shared.Helpers;
using Shared.Interfaces;
using Sora.Enums;

namespace Sora.Objects
{
    public static class MultiplayerLobby
    {
        private static long _matches = -1;
        public static Dictionary<long, MultiplayerRoom> Rooms = new Dictionary<long, MultiplayerRoom>();

        public static IEnumerable<MultiplayerRoom> GetRooms()
        {
            foreach (KeyValuePair<long, MultiplayerRoom> room in Rooms)
                yield return room.Value;
        }
        
        public static void Add(MultiplayerRoom room)
        {
            room.MatchId = _matches;
            Rooms.Add(_matches++, room);
        }

        public static void Remove(long matchId)
        {
            if (Rooms.ContainsKey(matchId)) return;
            Rooms.Remove(matchId);
        }
    }

    public class MultiplayerSlot
    {
        public MultiSlotStatus Status;
        public int UserId;
        public MultiSlotTeam Team;
        public Mod Mods;

        public override string ToString()
        {
            return $"Status: {Status} UserId: {UserId} Team: {Team} Mods: {Mods}";
        }
    }
    
    public class MultiplayerRoom : ISerializer
    {
        private const int MaxPlayers = 16;
        
        public long MatchId;
        public bool InProgress;
        public MatchType MatchType;
        public Mod ActiveMods;
        public string Name;
        public string Password;
        public string BeatmapName;
        public int BeatmapId;
        public string BeatmapMd5;
        public MultiplayerSlot[] Slots = new MultiplayerSlot[16];
        public int HostId;
        public PlayMode PlayMode;
        public ScoringType ScoringType;
        public TeamType TeamType;
        public byte SpecialModes;
        public int Seed;

        public MultiplayerRoom()
        {
            for (int i = 0; i < MaxPlayers; i++)
            {
                Slots[i] = new MultiplayerSlot
                {
                    // ReSharper disable once ConditionalTernaryEqualBranch
                    Status = i > 6 ? MultiSlotStatus.Locked : MultiSlotStatus.Locked,
                    Mods   = 0,
                    Team   = MultiSlotTeam.NoTeam,
                    UserId = -1
                };
            }
        }

        #region ISerializer
        public void ReadFromStream(MStreamReader sr)
        {
            MatchId = sr.ReadInt16();
            InProgress = sr.ReadBoolean();
            MatchType = (MatchType) sr.ReadByte();
            ActiveMods = (Mod) sr.ReadUInt32();
            Name = sr.ReadString();
            Password = sr.ReadString();
            BeatmapName = sr.ReadString();
            BeatmapId = sr.ReadInt32();
            BeatmapMd5 = sr.ReadString();

            for (int i = 0; i < MaxPlayers; i++)
                Slots[i].Status = (MultiSlotStatus) sr.ReadByte();
            
            for (int i = 0; i < MaxPlayers; i++)
                Slots[i].Team = (MultiSlotTeam) sr.ReadByte();
            
            for (int i = 0; i < MaxPlayers; i++)
                Slots[i].UserId = (Slots[i].Status & (MultiSlotStatus) 124) > 0 ? sr.ReadInt32() : -1;

            HostId = sr.ReadInt32();
            PlayMode = (PlayMode) sr.ReadByte();
            ScoringType = (ScoringType) sr.ReadByte();
            TeamType = (TeamType) sr.ReadByte();
            SpecialModes = sr.ReadByte();
            
            if ((SpecialModes & 0x1) > 0)
                for (int i = 0; i < MaxPlayers; i++)
                    Slots[i].Mods = (Mod) sr.ReadUInt32();
            
            Seed = sr.ReadInt32();
        }
        
        public void WriteToStream(MStreamWriter sw)
        {
            sw.Write((short)MatchId);
            sw.Write(InProgress);

            sw.Write((byte)MatchType);
            sw.Write((uint) ActiveMods);

            sw.Write(Name);
            sw.Write(Password.Length > 0 ? Crypto.RandomString(16) : null, true);

            sw.Write(BeatmapName);
            sw.Write(BeatmapId);
            sw.Write(BeatmapMd5);

            foreach (MultiplayerSlot slot in Slots)
                sw.Write((byte) slot.Status);

            foreach (MultiplayerSlot slot in Slots)
                sw.Write((byte) slot.Team);

            foreach (MultiplayerSlot slot in Slots)
                if ((slot.Status & (MultiSlotStatus)0x7c) > 0)
                    sw.Write(slot.UserId);

            sw.Write(HostId);
            sw.Write((byte) PlayMode);
            sw.Write((byte) ScoringType);
            sw.Write((byte) TeamType);
            sw.Write(SpecialModes);

            if (SpecialModes > 0)
                foreach (MultiplayerSlot slot in Slots)
                    sw.Write((uint) slot.Mods);

            sw.Write(Seed);
        }
        #endregion
        
        #region Default Stuff

        public override string ToString()
        {
            StringBuilder rawSlot = new StringBuilder();
            foreach (MultiplayerSlot slot in Slots)
                rawSlot.Append($"\t{slot}\n");
            
            return $"MatchId: {MatchId}\n" +
                   $"InProgress: {InProgress}\n" +
                   $"MatchType: {MatchType}\n" +
                   $"ActiveMods: {ActiveMods}\n" +
                   $"Name: {Name}\n" +
                   $"Password: {Password}\n" +
                   $"BeatmapName: {BeatmapName}\n" +
                   $"BeatmapId: {BeatmapId}\n" +
                   $"BeatmapMD5: {BeatmapMd5}\n" +
                   $"Slots: [\n{rawSlot}]\n" +
                   $"HostId: {HostId}\n" +
                   $"PlayMode: {PlayMode}\n" +
                   $"ScoringType: {ScoringType}\n" +
                   $"TeamType: {TeamType}\n" +
                   $"SpecialModes: {SpecialModes}\n" +
                   $"Seed: {Seed}";
        }

        #endregion

        #region Multiplayer Stuff

        public bool Join(Presence pr)
        {
            MultiplayerSlot slot = Slots.First(x => x.UserId == -1 && x.Status == MultiSlotStatus.Open || x.Status == MultiSlotStatus.Quit);
            if (slot == null) return false;
            slot.UserId = pr.User.Id;
            slot.Status = MultiSlotStatus.NotReady;
            pr.JoinedRoom = this;
            return true;
        }

        public bool Leave(Presence pr)
        {
            MultiplayerSlot slot = Slots.First(x => x.UserId == pr.User.Id && x.Status != MultiSlotStatus.Open ||
                                                    x.Status == MultiSlotStatus.Quit);
            
            if (slot == null) return false;
            
            slot.UserId = -1;
            slot.Status = MultiSlotStatus.Open;
            slot.Team = MultiSlotTeam.NoTeam;
            slot.Mods = Mod.None;
            pr.JoinedRoom = null;
            return true;
        }
        
        #endregion
    }
}