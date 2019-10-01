using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Sora.Framework.Allocation;
using Sora.Framework.Enums;
using Sora.Framework.Packets.Server;
using Sora.Framework.Utilities;

namespace Sora.Framework.Objects.Multiplayer
{
    public class MatchSlot : DynamicValues
    {
        public Mod Mods;
        public MultiSlotStatus Status;
        public MultiSlotTeam Team;
        public int UserId;

        public override string ToString() => $"Status: {Status} UserId: {UserId} Team: {Team} Mods: {Mods}";

        public bool IsHost(Match room) => room.HostId == UserId;
    }

    public class MultiplayerException : Exception
    {
        public MultiplayerException(string s)
            : base(s)
        {
        }
    }

    public class Match : PresenceKeeper, ISerializer
    {
        private const int MaxPlayers = 16;
        public Mod ActiveMods;
        public int BeatmapId;
        public string BeatmapMd5;
        public string BeatmapName;

        public Channel Channel;
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
        public MatchSlot[] Slots = new MatchSlot[16];
        public MatchSpecialModes SpecialModes;
        public TeamType TeamType;
        public int WantSkip;

        public Match()
        {
            Channel = new Channel
            {
                Name = "#multiplayer",
                Topic = "Even more osu! default channels!"
            };
            
            for (var i = 0; i < MaxPlayers; i++)
                Slots[i] = new MatchSlot
                {
                    // ReSharper disable once ConditionalTernaryEqualBranch
                    Status = i > 6 ? MultiSlotStatus.Locked : MultiSlotStatus.Locked,
                    Mods = 0,
                    Team = MultiSlotTeam.NoTeam,
                    UserId = -1
                };
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

            for (var i = 0; i < MaxPlayers; i++)
                Slots[i].Status = (MultiSlotStatus) sr.ReadByte();

            for (var i = 0; i < MaxPlayers; i++)
                Slots[i].Team = (MultiSlotTeam) sr.ReadByte();

            for (var i = 0; i < MaxPlayers; i++)
                Slots[i].UserId = (Slots[i].Status & (MultiSlotStatus) 124) > 0 ? sr.ReadInt32() : -1;

            HostId = sr.ReadInt32();
            PlayMode = (PlayMode) sr.ReadByte();
            ScoringType = (ScoringType) sr.ReadByte();
            TeamType = (TeamType) sr.ReadByte();
            SpecialModes = (MatchSpecialModes) sr.ReadByte();

            if (SpecialModes == MatchSpecialModes.Freemods)
                for (var i = 0; i < MaxPlayers; i++)
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

            foreach (var slot in Slots)
                sw.Write((byte) slot.Status);

            foreach (var slot in Slots)
                sw.Write((byte) slot.Team);

            foreach (var slot in Slots)
                if ((slot.Status & (MultiSlotStatus) 0x7c) > 0)
                    sw.Write(slot.UserId);

            sw.Write(HostId);
            sw.Write((byte) PlayMode);
            sw.Write((byte) ScoringType);
            sw.Write((byte) TeamType);
            sw.Write((byte) SpecialModes);

            if (SpecialModes == MatchSpecialModes.Freemods)
                foreach (var slot in Slots)
                    sw.Write((uint) slot.Mods);

            sw.Write(Seed);
        }

        #endregion

        #region Default Stuff

        public override string ToString()
        {
            var rawSlot = new StringBuilder();
            foreach (var slot in Slots)
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

        public override void Join(Presence pr)
        {
            if (!Join(pr, null))
                throw new MultiplayerException("Password is Incorrect!");
        }

        public bool Join(Presence pr, string password)
        {
            if (Password.Trim() != password.Trim())
                return false;
            
            var slot =
                Slots.First(
                    x => x.UserId == -1 && x.Status == MultiSlotStatus.Open ||
                         x.Status == MultiSlotStatus.Quit
                );
            if (slot == null)
                return false;
            
            slot.UserId = pr.User.Id;
            slot.Status = MultiSlotStatus.NotReady;
            
            Channel.Join(pr);
            pr.ActiveMatch = this;
            base.Join(pr);
            return true;
        }

        public override void Leave(Presence pr)
        {
            var slot = Slots.First(x => x.UserId == pr.User.Id);
            if (slot == null)
                return;
            
            ClearSlot(slot);
            
            Channel.Leave(pr);
            base.Leave(pr);
            
            pr.ActiveMatch = null;
        }

        public void Leave(int userId)
        {
            var slot = Slots.First(x => x.UserId == userId);
            if (slot == null)
                return;
            
            ClearSlot(slot);
        }

        [SuppressMessage("ReSharper", "RedundantAssignment")]
        public void Invite(Presence sender, Presence receiver, string msg = "Hey, I want to play with you! Join me [%INVITE_URI% %MATCH_NAME%]")
        {
            // Took me a while to figure out. i tried osu://mp/matchid/Password.Replace(" ", "_");
            var inviteUri = $"osump://{MatchId}/{Password.Replace(" ", "_")}";

            receiver.Push(new Invite(
                new MessageStruct
                {
                    ChannelTarget = sender.User.UserName,
                    Message = $"\0{msg}".Replace("%INVITE_URI%", inviteUri).Replace("%MATCH_NAME%", Name),
                    Username = sender.User.UserName,
                    SenderId = sender.User.Id
                }
            ));
        }

        public void SetSlot(
            int slotId, int userId, MultiSlotStatus status,
            MultiSlotTeam team = MultiSlotTeam.NoTeam,
            Mod mods = Mod.None)
        {
            if (slotId > MaxPlayers)
                throw new ArgumentOutOfRangeException();

            var slot = Slots[slotId];
            slot.UserId = userId;
            slot.Status = status;
            slot.Team = team;
            slot.Mods = mods;
        }

        public void SetSlot(int slotId, MatchSlot slot)
        {
            if (slotId > MaxPlayers)
                throw new ArgumentOutOfRangeException();

            var firstSlot = Slots[slotId];
            firstSlot.UserId = slot.UserId;
            firstSlot.Status = slot.Status;
            firstSlot.Team = slot.Team;
            firstSlot.Mods = slot.Mods;
        }

        public void SetSlot(MatchSlot firstSlot, MatchSlot secondSlot)
        {
            firstSlot.UserId = secondSlot.UserId;
            firstSlot.Status = secondSlot.Status;
            firstSlot.Team = secondSlot.Team;
            firstSlot.Mods = secondSlot.Mods;
        }

        public void ClearSlot(MatchSlot slot)
        {
            slot.UserId = -1;
            slot.Status = MultiSlotStatus.Open;
            slot.Team = MultiSlotTeam.NoTeam;
            slot.Mods = Mod.None;
        }

        public void Update()
        {
            Push(new MatchUpdate(this));
            Lobby.Self.Push(new MatchUpdate(this));
        }

        public void Disband()
        {
            Lobby.Self.Pop(this);
            Push(new MatchDisband(this));
            Lobby.Self.Push(new MatchDisband(this));
        }

        public void SetHost(int userId)
        {
            HostId = userId;
        }

        public void SetRandomHost()
        {
            var slot = Slots.FirstOrDefault(x => x.UserId != -1);
            if (slot != null)
                HostId = slot.UserId;
            else
                HostId = -1;
        }

        public void SetMods(Mod mods, MatchSlot slot)
        {
            if (slot.IsHost(this) && SpecialModes == MatchSpecialModes.Freemods)
            {
                slot.Mods = FixMods(mods);
            }
            else if (SpecialModes == MatchSpecialModes.Freemods)
            {
                slot.Mods = mods;
            }
            else if (slot.IsHost(this) && SpecialModes == MatchSpecialModes.Normal)
            {
                foreach (var s in Slots)
                    slot.Mods = Mod.None;

                ActiveMods = mods;
            }
        }

        public void ChangeSettings(Match room)
        {
            MatchType = room.MatchType;
            ActiveMods = room.ActiveMods;
            Name = room.Name;
            BeatmapName = room.BeatmapName;
            BeatmapId = room.BeatmapId;
            BeatmapMd5 = room.BeatmapMd5;
            HostId = room.HostId;
            PlayMode = room.PlayMode;
            ScoringType = room.ScoringType;
            TeamType = room.TeamType;
            SpecialModes = room.SpecialModes;
            Seed = room.Seed;

            SetMods(ActiveMods, GetSlotByUserId(HostId));
        }

        public void SetPassword(string password)
        {
            Password = password.Replace(" ", "_");
        }

        public void LockSlot(MatchSlot slot)
        {
            if (slot.UserId != -1)
                Leave(slot.UserId);
            slot.Mods = Mod.None;

            slot.Status = slot.Status != MultiSlotStatus.Locked
                ? MultiSlotStatus.Locked
                : MultiSlotStatus.Open;

            slot.Team = MultiSlotTeam.NoTeam;
        }

        public void Start()
        {
            InProgress = true;

            foreach (var slot in Slots.Where(
                x => x.UserId != -1 && x.Status != MultiSlotStatus.NoMap
            ))
            {
                slot.Status = MultiSlotStatus.Playing;
                ++NeedLoad;
                ++PlayingPeople;
            }

            Push(new MatchStart(this));
        }

        public void LoadComplete()
        {
            if (--NeedLoad == 0)
                Push(new MatchAllPlayersLoaded());
        }

        public void Failed(Presence pr)
        {
            Failed(pr.User.Id);
        }

        public void Failed(int userId)
        {
            FailedPeople++;
            Push(new MatchPlayerFailed(GetSlotIdByUserId(userId)));
        }

        public void Complete(Presence pr)
        {
            Complete(pr.User.Id);
        }

        public void Complete(int userId)
        {
            PlayingPeople--;

            var slot = GetSlotByUserId(userId);
            slot.Status = MultiSlotStatus.Complete;

            if (PlayingPeople != 0)
                return;

            foreach (var slt in Slots.Where(x => x.Status == MultiSlotStatus.Complete))
                slt.Status = MultiSlotStatus.NotReady;

            InProgress = false;
            Push(new MatchComplete());
        }

        public void Skip(Presence pr)
        {
            Skip(pr.User.Id);
        }

        public void Skip(int userId)
        {
            WantSkip++;
            Push(new MatchPlayerSkipped(GetSlotIdByUserId(userId)));
            if (WantSkip == PlayingPeople)
                Push(new MatchSkip());
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

        public MatchSlot GetSlotByUserId(int userId)
        {
            return Slots.FirstOrDefault(x => x.UserId == userId);
        }

        public int GetSlotIdByUserId(int userId)
        {
            var result = Slots
                         .Select((v, i) => new {v, i})
                         .Where(s => s.v.UserId == userId)
                         .Select(h => h.i)
                         .FirstOrDefault();

            return result;
        }

        #endregion
    }
}