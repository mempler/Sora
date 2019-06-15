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
using System.Linq;
using System.Text;
using Sora.Enums;
using Sora.Packets.Server;
using Sora.Services;
using Crypto = Sora.Helpers.Crypto;
using IPacket = Sora.Interfaces.IPacket;
using ISerializer = Sora.Interfaces.ISerializer;
using Mod = Sora.Enums.Mod;
using MStreamReader = Sora.Helpers.MStreamReader;
using MStreamWriter = Sora.Helpers.MStreamWriter;
using PlayMode = Sora.Enums.PlayMode;

namespace Sora.Objects
{
    public class MultiplayerSlot
    {
        public Mod Mods;
        public MultiSlotStatus Status;
        public MultiSlotTeam Team;
        public int UserId;

        public override string ToString()
        {
            return $"Status: {Status} UserId: {UserId} Team: {Team} Mods: {Mods}";
        }

        public bool IsHost(MultiplayerRoom room)
        {
            return room.HostId == UserId;
        }
    }

    public class MultiplayerRoom : ISerializer, ICloneable
    {
        public PacketStreamService _pss;
        public MultiplayerService _ms;
        public PresenceService _ps;

        private const int MaxPlayers = 16;
        public Mod ActiveMods;
        public int BeatmapId;
        public string BeatmapMd5;
        public string BeatmapName;

        public Channel Channel = new Channel("#multiplayer", "Even more osu! default channels!");
        public int FailedPeople;
        public int HostId;
        public bool InProgress;

        public long MatchId;
        public MatchType MatchType;
        public string Name;
        public int NeedLoad;
        public string Password;
        public int PlayingPeople;
        public PlayMode PlayMode;
        public ScoringType ScoringType;
        public int Seed;
        public MultiplayerSlot[] Slots = new MultiplayerSlot[16];
        public MatchSpecialModes SpecialModes;
        public TeamType TeamType;
        public int WantSkip;

        public MultiplayerRoom(PacketStreamService pss, MultiplayerService ms, PresenceService ps)
        {
            _pss = pss;
            _ms = ms;
            _ps = ps;
            for (int i = 0; i < MaxPlayers; i++)
                Slots[i] = new MultiplayerSlot
                {
                    // ReSharper disable once ConditionalTernaryEqualBranch
                    Status = i > 6 ? MultiSlotStatus.Locked : MultiSlotStatus.Locked,
                    Mods   = 0,
                    Team   = MultiSlotTeam.NoTeam,
                    UserId = -1
                };
        }

        public MultiplayerRoom()
        {
            for (int i = 0; i < MaxPlayers; i++)
                Slots[i] = new MultiplayerSlot
                {
                    // ReSharper disable once ConditionalTernaryEqualBranch
                    Status = i > 6 ? MultiSlotStatus.Locked : MultiSlotStatus.Locked,
                    Mods   = 0,
                    Team   = MultiSlotTeam.NoTeam,
                    UserId = -1
                };
        }

        #region ISerializer

        public void ReadFromStream(MStreamReader sr)
        {
            MatchId     = sr.ReadInt16();
            InProgress  = sr.ReadBoolean();
            MatchType   = (MatchType) sr.ReadByte();
            ActiveMods  = (Mod) sr.ReadUInt32();
            Name        = sr.ReadString();
            Password    = sr.ReadString();
            BeatmapName = sr.ReadString();
            BeatmapId   = sr.ReadInt32();
            BeatmapMd5  = sr.ReadString();

            for (int i = 0; i < MaxPlayers; i++)
                Slots[i].Status = (MultiSlotStatus) sr.ReadByte();

            for (int i = 0; i < MaxPlayers; i++)
                Slots[i].Team = (MultiSlotTeam) sr.ReadByte();

            for (int i = 0; i < MaxPlayers; i++)
                Slots[i].UserId = (Slots[i].Status & (MultiSlotStatus) 124) > 0 ? sr.ReadInt32() : -1;

            HostId       = sr.ReadInt32();
            PlayMode     = (PlayMode) sr.ReadByte();
            ScoringType  = (ScoringType) sr.ReadByte();
            TeamType     = (TeamType) sr.ReadByte();
            SpecialModes = (MatchSpecialModes) sr.ReadByte();

            if (SpecialModes == MatchSpecialModes.Freemods)
                for (int i = 0; i < MaxPlayers; i++)
                    Slots[i].Mods = (Mod) sr.ReadUInt32();

            Seed = sr.ReadInt32();
        }

        public void WriteToStream(MStreamWriter sw)
        {
            sw.Write((short) MatchId);
            sw.Write(InProgress);

            sw.Write((byte) MatchType);
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
                if ((slot.Status & (MultiSlotStatus) 0x7c) > 0)
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

        public object Clone()
        {
            return MemberwiseClone();
        }

        #endregion

        #region Multiplayer Stuff

        public bool Join(Presence pr, string password)
        {
            if (Password.Trim() != password.Trim()) return false;
            MultiplayerSlot slot =
                Slots.First(x => x.UserId == -1 && x.Status == MultiSlotStatus.Open ||
                                 x.Status == MultiSlotStatus.Quit);
            if (slot == null) return false;
            slot.UserId   = pr.User.Id;
            slot.Status   = MultiSlotStatus.NotReady;
            pr.JoinedRoom = this;
            return true;
        }

        public void Leave(Presence pr)
        {
            MultiplayerSlot slot = Slots.First(x => x.UserId == pr.User.Id);
            if (slot == null) return;
            ClearSlot(slot);
            pr.JoinedRoom = null;
        }

        public void Leave(int userId)
        {
            MultiplayerSlot slot = Slots.First(x => x.UserId == userId);
            if (slot == null) return;
            ClearSlot(slot);
        }

        public void Broadcast(IPacket packet)
        {
            foreach (MultiplayerSlot slot in Slots.Where(x => x.UserId != -1))
            {
                Presence pr = _ps.GetPresence(slot.UserId);
                if (pr == null)
                    Leave(slot.UserId);
                else
                    pr.Write(packet);
            }
        }

        public void Invite(Presence pr, Presence opr)
        {
            // Took me a while to figure out. i tried osu://mp/matchid/Password.Replace(" ", "_");
            string inviteUri = $"osump://{MatchId}/{Password.Replace(" ", "_")}";
    
            opr.Write(new Invite(new MessageStruct
            {
                ChannelTarget = pr.User.Username,
                Message       = $"\0Hey, I want to play with you! Join me [{inviteUri} {Name}]",
                Username      = pr.User.Username,
                SenderId      = opr.User.Id
            }));
        }

        public void SetSlot(
            int slotId, int userId, MultiSlotStatus status,
            MultiSlotTeam team = MultiSlotTeam.NoTeam,
            Mod mods = Mod.None)
        {
            if (slotId > MaxPlayers) throw new ArgumentOutOfRangeException();

            MultiplayerSlot slot = Slots[slotId];
            slot.UserId = userId;
            slot.Status = status;
            slot.Team   = team;
            slot.Mods   = mods;

            Update();
        }

        public void SetSlot(int slotId, MultiplayerSlot slot)
        {
            if (slotId > MaxPlayers) throw new ArgumentOutOfRangeException();

            MultiplayerSlot firstslot = Slots[slotId];
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
            _pss.GetStream("lobby").Broadcast(new MatchUpdate(this));
            Broadcast(new MatchUpdate(this));
        }

        public void Dispand()
        {
            Broadcast(new MatchDisband(this));
            _ms.Remove(MatchId);
            _pss.GetStream("lobby").Broadcast(new MatchDisband(this));
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

        public void SetMods(Mod mods, MultiplayerSlot slot)
        {
            if (slot.IsHost(this) && SpecialModes == MatchSpecialModes.Freemods)
                slot.Mods = FixMods(mods);
            else if (SpecialModes == MatchSpecialModes.Freemods)
                slot.Mods = mods;
            else if (slot.IsHost(this) && SpecialModes == MatchSpecialModes.Normal) {
                foreach (MultiplayerSlot s in Slots)
                    slot.Mods = Mod.None;
                
                ActiveMods = mods;
            }

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
            
            SetMods(ActiveMods, GetSlotByUserId(HostId));
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

            Update();
            Broadcast(new MatchStart(this));
        }

        public void LoadComplete()
        {
            if (--NeedLoad == 0)
                Broadcast(new MatchAllPlayersLoaded());

            Update();
        }

        public void Failed(Presence pr)
        {
            Failed(pr.User.Id);
        }

        public void Failed(int userId)
        {
            FailedPeople++;
            Broadcast(new MatchPlayerFailed(GetSlotIdByUserId(userId)));
        }

        public void Complete(Presence pr)
        {
            Complete(pr.User.Id);
        }

        public void Complete(int userId)
        {
            PlayingPeople--;

            MultiplayerSlot slot = GetSlotByUserId(userId);
            slot.Status = MultiSlotStatus.Complete;
            Update();

            if (PlayingPeople != 0) return;

            foreach (MultiplayerSlot slt in Slots.Where(x => x.Status == MultiSlotStatus.Complete))
                slt.Status = MultiSlotStatus.NotReady;

            InProgress = false;
            Update();
            Broadcast(new MatchComplete());
        }

        public void Skip(Presence pr)
        {
            Skip(pr.User.Id);
        }

        public void Skip(int userId)
        {
            WantSkip++;
            Broadcast(new MatchPlayerSkipped(GetSlotIdByUserId(userId)));
            if (WantSkip == PlayingPeople)
                Broadcast(new MatchSkip());
        }

        private Mod FixMods(Mod mods)
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

        public MultiplayerSlot GetSlotByUserId(int userId)
        {
            return Slots.FirstOrDefault(x => x.UserId == userId);
        }

        public int GetSlotIdByUserId(int userId)
        {
            int result = Slots
                         .Select((v, i) => new {v, i})
                         .Where(s => s.v.UserId == userId)
                         .Select(h => h.i)
                         .FirstOrDefault();

            return result;
        }

        #endregion
    }
}