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
using System.Linq;
using System.Text;
using Shared.Enums;
using Shared.Helpers;
using Shared.Interfaces;
using Sora.Enums;
using Sora.Packets.Server;

namespace Sora.Objects
{
    public static class MultiplayerLobby
    {
        private static long _matches;
        public static Dictionary<long, MultiplayerRoom> Rooms = new Dictionary<long, MultiplayerRoom>();

        public static IEnumerable<MultiplayerRoom> GetRooms() => Rooms.Select(x => x.Value);

        public static MultiplayerRoom GetRoom(long matchId)
        {
            return Rooms.ContainsKey(matchId) ? Rooms[matchId] : null;
        }

        public static long Add(MultiplayerRoom room)
        {
            room.MatchId = _matches++;
            Rooms.Add(room.MatchId, room);
            return room.MatchId;
        }

        public static void Remove(long matchId)
        {
            if (!Rooms.ContainsKey(matchId)) return;
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

        public bool IsHost(MultiplayerRoom room) => room.HostId == UserId;
    }
    
    public class MultiplayerRoom : ISerializer, ICloneable
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
        public MatchSpecialModes SpecialModes;
        public int Seed;
        public int NeedLoad;
        public int PlayingPeople;
        
        public Channel Channel = new Channel("#multiplayer");

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
            SpecialModes = (MatchSpecialModes) sr.ReadByte();
            
            if (SpecialModes == MatchSpecialModes.Freemods)
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
            sw.Write((byte) SpecialModes);

            if (SpecialModes == MatchSpecialModes.Freemods)
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

        public object Clone() => MemberwiseClone();
        #endregion

        #region Multiplayer Stuff

        public bool Join(Presence pr, string password)
        {
            if (Password.Trim() != password.Trim()) return false;
            MultiplayerSlot slot = Slots.First(x => x.UserId == -1 && x.Status == MultiSlotStatus.Open || x.Status == MultiSlotStatus.Quit);
            if (slot == null) return false;
            slot.UserId = pr.User.Id;
            slot.Status = MultiSlotStatus.NotReady;
            pr.JoinedRoom = this;
            return true;
        }

        public bool Leave(Presence pr)
        {
            MultiplayerSlot slot = Slots.First(x => x.UserId == pr.User.Id);
            
            if (slot == null) return false;
            
            slot.UserId = -1;
            slot.Status = MultiSlotStatus.Quit;
            slot.Team = MultiSlotTeam.NoTeam;
            slot.Mods = Mod.None;
            pr.JoinedRoom = null;
            return true;
        }

        public bool Leave(int userId)
        {
            MultiplayerSlot slot = Slots.First(x => x.UserId == userId);

            if (slot == null) return false;

            slot.UserId   = -1;
            slot.Status   = MultiSlotStatus.Open;
            slot.Team     = MultiSlotTeam.NoTeam;
            slot.Mods     = Mod.None;
            return true;
        }
        
        public void Broadcast(IPacket packet)
        {
            foreach (MultiplayerSlot slot in Slots.Where(x => x.UserId != -1))
            {
                Presence pr = LPresences.GetPresence(slot.UserId);
                if (pr == null)
                    Leave(slot.UserId);
                else
                    pr.Write(packet);
            }
        }

        public void Invite(Presence pr)
        {
            // Took me a while to figure out. i tried osu://mp/matchid/Password.Replace(" ", "_");
            string inviteUri = $"osump://{MatchId}/{Password.Replace(" ", "_")}";
            
            pr.Write(new Invite(new MessageStruct
            {
                ChannelTarget = pr.User.Username,
                Message       = $"\0Hey, I want to play with you! Join me [{inviteUri} {Name}]",
                Username      = pr.User.Username,
                SenderId      = pr.User.Id
            }));
        }

        public void SetSlot(int SlotId, int userId, MultiSlotStatus status,
                            MultiSlotTeam team = MultiSlotTeam.NoTeam,
                            Mod mods = Mod.None)
        {
            if (SlotId > MaxPlayers) throw new ArgumentOutOfRangeException();

            MultiplayerSlot slot = Slots[SlotId];
            slot.UserId = userId;
            slot.Status = status;
            slot.Team = team;
            slot.Mods = mods;
            
            Update();
        }

        public void SetSlot(int SlotId, MultiplayerSlot slot)
        {
            if (SlotId > MaxPlayers) throw new ArgumentOutOfRangeException();

            MultiplayerSlot firstslot = Slots[SlotId];
            firstslot.UserId = slot.UserId;
            firstslot.Status = slot.Status;
            firstslot.Team   = slot.Team;
            firstslot.Mods   = slot.Mods;
            
            Update();
        }

        public void SetSlot(MultiplayerSlot firstslot, MultiplayerSlot secondslot)
        {
            firstslot.UserId = secondslot.UserId;
            firstslot.Status = secondslot.Status;
            firstslot.Team   = secondslot.Team;
            firstslot.Mods   = secondslot.Mods;
            
            Update();
        }

        public void ClearSlot(MultiplayerSlot slot)
        {
            slot.UserId = -1;
            slot.Status = MultiSlotStatus.Open;
            slot.Team   = MultiSlotTeam.NoTeam;
            slot.Mods   = Mod.None;
            
            Update();
        }

        public void Update()
        {
            LPacketStreams.GetStream("lobby").Broadcast(new MatchUpdate(this));
            Broadcast(new MatchUpdate(this));
        }

        public void Dispand()
        {            
            Broadcast(new MatchDisband(this));
            MultiplayerLobby.Remove(MatchId);
            LPacketStreams.GetStream("lobby").Broadcast(new MatchDisband(this));
        }

        public void SetHost(int userId)
        {
            HostId = userId;
            Update();
        }

        public void SetRandomHost()
        {
            MultiplayerSlot slot = Slots.FirstOrDefault(x => x.UserId != -1);
            if (slot != null)
                HostId = slot.UserId;
            else
                HostId = -1;

            Update();
        }

        public void SetMods(Mod mods, MultiplayerSlot Slot)
        {
            if (Slot.IsHost(this) && SpecialModes == MatchSpecialModes.Freemods) {
                Slot.Mods = fixMods(mods);
            } else if (SpecialModes == MatchSpecialModes.Freemods)
                Slot.Mods = mods;
            else if (Slot.IsHost(this) && SpecialModes == MatchSpecialModes.Normal)
                ActiveMods = mods;

            Update();
        }

        public void ChangeSettings(MultiplayerRoom room)
        {
            MatchType    = room.MatchType;
            ActiveMods   = room.ActiveMods;
            Name         = room.Name;
            BeatmapName  = room.BeatmapName;
            BeatmapId    = room.BeatmapId;
            BeatmapMd5   = room.BeatmapMd5;
            HostId       = room.HostId;
            PlayMode     = room.PlayMode;
            ScoringType  = room.ScoringType;
            TeamType     = room.TeamType;
            SpecialModes = room.SpecialModes;
            Seed         = room.Seed;
            
            Update();
        }

        public void SetPassword(string password)
        {
            Password = password.Replace(" ", "_");
            Update();
        }

        public void LockSlot(MultiplayerSlot slot)
        {
            if (slot.UserId != -1)
                Leave(slot.UserId);
            slot.Mods = Mod.None;

            slot.Status = slot.Status != MultiSlotStatus.Locked
                ? MultiSlotStatus.Locked
                : MultiSlotStatus.Open;

            slot.Team = MultiSlotTeam.NoTeam;
            
            Update();
        }

        public void Start()
        {
            InProgress = true;

            foreach (MultiplayerSlot slot in Slots.Where(
                x => x.UserId != -1 && x.Status != MultiSlotStatus.NoMap))
            {
                slot.Status = MultiSlotStatus.Playing;
                ++NeedLoad;
                ++PlayingPeople;
            }

            Broadcast(new MatchStart(this));
            Update();
        }

        public void LoadComplete()
        {
            if (--NeedLoad == 0)
                Broadcast(new MatchAllPlayersLoaded());

            Update();
        }
        
        private Mod fixMods(Mod mods)
        {
            ActiveMods = 0;
            if ((mods & Mod.DoubleTime) > 0)
                ActiveMods |= Mod.DoubleTime;
            if ((mods & Mod.Nightcore) > 0)
                ActiveMods |= Mod.Nightcore;
            if ((mods & Mod.HalfTime) > 0)
               ActiveMods |= Mod.HalfTime;
                
            return mods;
        }
        
        #endregion
    }
}