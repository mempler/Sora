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
using Shared.Enums;
using Shared.Handlers;
using Sora.Enums;
using Sora.Objects;
using Sora.Packets.Server;

namespace Sora.Handler
{
    public class MultiplayerHandler
    {
        #region Lobby
        [Handler(HandlerTypes.ClientLobbyJoin)]
        public void OnLobbyJoin(Presence pr)
        {
            PacketStream lobbyStream = LPacketStreams.GetStream("lobby");
            IEnumerable<MultiplayerRoom> rooms = MultiplayerLobby.GetRooms();
            foreach (MultiplayerRoom room in rooms)
                pr.Write(new MatchNew(room));
            
            lobbyStream.Join(pr);
        }
        
        [Handler(HandlerTypes.ClientLobbyPart)]
        public void OnLobbyLeft(Presence pr)
        {
            LPacketStreams.GetStream("lobby").Left(pr);
        }
        #endregion

        #region Match
        [Handler(HandlerTypes.ClientMatchCreate)]
        public void OnMatchCreate(Presence pr, MultiplayerRoom room)
        {
            room.Password = room.Password.Replace(" ", "_");
            MultiplayerLobby.Add(room);
            if (room.Join(pr, room.Password))
                pr.Write(new MatchJoinSuccess(room));
            else
                pr.Write(new MatchJoinFail());
            LPacketStreams.GetStream("lobby").Broadcast(new MatchNew(room));
        }

        [Handler(HandlerTypes.ClientMatchPart)]
        public void OnMatchLeave(Presence pr)
        {
            if (pr.JoinedRoom == null) return;
            
            MultiplayerRoom room = pr.JoinedRoom;
            if (room.HostId == pr.User.Id)
            {
                MultiplayerSlot slot = room.Slots.FirstOrDefault(x => x.UserId != pr.User.Id && x.UserId != -1);
                if (slot != null)
                    room.HostId = slot.UserId;
                else
                    room.HostId = -1;
            }

            room.Leave(pr);
            if (room.HostId != -1)
            {
                LPacketStreams.GetStream("lobby").Broadcast(new MatchUpdate(room));
                room.Broadcast(new MatchUpdate(room));
                return;
            }

            MultiplayerLobby.Remove(room.MatchId);
            LPacketStreams.GetStream("lobby").Broadcast(new MatchDisband(room));
        }

        [Handler(HandlerTypes.ClientMatchJoin)]
        public void OnMatchJoin(Presence pr, int matchId, string password)
        {
            MultiplayerRoom room = MultiplayerLobby.GetRoom(matchId);
            if (room != null && room.Join(pr, password.Replace(" ", "_")))
                pr.Write(new MatchJoinSuccess(room));
            else
                pr.Write(new MatchJoinFail());

            LPacketStreams.GetStream("lobby").Broadcast(new MatchUpdate(room));
            room?.Broadcast(new MatchUpdate(room));
        }

        [Handler(HandlerTypes.ClientMatchChangeSlot)]
        public void OnMatchChangeSlot(Presence pr, int slotId)
        {
            if (pr.JoinedRoom == null) return;
            if (slotId > 16) return;
            MultiplayerSlot oldSlot = pr.JoinedRoom.Slots.First(x => x.UserId == pr.User.Id);
            MultiplayerSlot newSlot = pr.JoinedRoom.Slots[slotId];
            if (newSlot.UserId != -1) return;

            newSlot.UserId = oldSlot.UserId;
            newSlot.Status = oldSlot.Status;
            newSlot.Team = oldSlot.Team;
            newSlot.Mods = oldSlot.Mods;
            
            oldSlot.UserId = -1;
            oldSlot.Status = MultiSlotStatus.Open;
            oldSlot.Team   = MultiSlotTeam.NoTeam;
            oldSlot.Mods   = Mod.None;

            LPacketStreams.GetStream("lobby").Broadcast(new MatchUpdate(pr.JoinedRoom));
            pr.JoinedRoom.Broadcast(new MatchUpdate(pr.JoinedRoom));
        }

        [Handler(HandlerTypes.ClientMatchChangeMods)]
        public void OnMatchChangeMods(Presence pr, Mod mods)
        {
            if (pr.JoinedRoom == null)
                return;
            
            if (pr.JoinedRoom.SpecialModes == MatchSpecialModes.Freemods)
            {
                MultiplayerSlot slot = pr.JoinedRoom.Slots.First(x => x.UserId == pr.User.Id);
                if (slot == null) return;
                slot.Mods = mods;
                if (pr.JoinedRoom.HostId == pr.User.Id)
                {
                    pr.JoinedRoom.ActiveMods = 0;
                    if ((mods & Mod.DoubleTime) > 0)
                        pr.JoinedRoom.ActiveMods |= Mod.DoubleTime;
                    if ((mods & Mod.Nightcore) > 0)
                        pr.JoinedRoom.ActiveMods |= Mod.Nightcore;
                    if ((mods & Mod.HalfTime) > 0)
                        pr.JoinedRoom.ActiveMods |= Mod.HalfTime;
                }
            } else if (pr.JoinedRoom.HostId == pr.User.Id && pr.JoinedRoom.SpecialModes == MatchSpecialModes.Normal)
            {
                pr.JoinedRoom.ActiveMods = mods;
            }

            LPacketStreams.GetStream("lobby").Broadcast(new MatchUpdate(pr.JoinedRoom));
            pr.JoinedRoom.Broadcast(new MatchUpdate(pr.JoinedRoom));
        }
        
        [Handler(HandlerTypes.ClientMatchChangeSettings)]
        public void OnMatchChangeSettings(Presence pr, MultiplayerRoom room)
        {
            if (pr.JoinedRoom == null) return;
            if (pr.JoinedRoom.HostId != pr.User.Id) return;

            pr.JoinedRoom.MatchType = room.MatchType;
            pr.JoinedRoom.ActiveMods = room.ActiveMods;
            pr.JoinedRoom.Name = room.Name;
            pr.JoinedRoom.BeatmapName = room.BeatmapName;
            pr.JoinedRoom.BeatmapId = room.BeatmapId;
            pr.JoinedRoom.BeatmapMd5 = room.BeatmapMd5;
            pr.JoinedRoom.HostId = room.HostId;
            pr.JoinedRoom.PlayMode = room.PlayMode;
            pr.JoinedRoom.ScoringType = room.ScoringType;
            pr.JoinedRoom.TeamType = room.TeamType;
            pr.JoinedRoom.SpecialModes = room.SpecialModes;
            pr.JoinedRoom.Seed = room.Seed;
            
            LPacketStreams.GetStream("lobby").Broadcast(new MatchUpdate(pr.JoinedRoom));
            pr.JoinedRoom.Broadcast(new MatchUpdate(pr.JoinedRoom));
        }

        [Handler(HandlerTypes.ClientMatchChangePassword)]
        public void OnMatchChangePassword(Presence pr, MultiplayerRoom room)
        {
            if (pr.JoinedRoom == null) return;
            if (pr.JoinedRoom.HostId != pr.User.Id) return;

            pr.JoinedRoom.Password    = room.Password.Replace(" ", "_");

            LPacketStreams.GetStream("lobby").Broadcast(new MatchUpdate(pr.JoinedRoom));
            pr.JoinedRoom.Broadcast(new MatchUpdate(pr.JoinedRoom));
        }
        
        [Handler(HandlerTypes.ClientMatchLock)]
        public void OnMatchLock(Presence pr, int slotId)
        {
            if (pr.JoinedRoom == null || pr.JoinedRoom.HostId != pr.User.Id) return;
            if (slotId > 16) return;

            if (pr.JoinedRoom.Slots[slotId].UserId != -1)
                pr.JoinedRoom.Leave(pr.JoinedRoom.Slots[slotId].UserId);
            pr.JoinedRoom.Slots[slotId].Mods = Mod.None;
            
            pr.JoinedRoom.Slots[slotId].Status = pr.JoinedRoom.Slots[slotId].Status != MultiSlotStatus.Locked ?
                MultiSlotStatus.Locked : MultiSlotStatus.Open;
            
            pr.JoinedRoom.Slots[slotId].Team = MultiSlotTeam.NoTeam;
            
            LPacketStreams.GetStream("lobby").Broadcast(new MatchUpdate(pr.JoinedRoom));
            pr.JoinedRoom.Broadcast(new MatchUpdate(pr.JoinedRoom));
        }

        [Handler(HandlerTypes.ClientMatchChangeTeam)]
        public void OnMatchChangeTeam(Presence pr)
        {
            MultiplayerSlot slot = pr.JoinedRoom?.Slots.First(x => pr.User.Id == x.UserId);
            if (slot == null) return;

            switch (slot.Team)
            {
                case MultiSlotTeam.Blue:
                    slot.Team = MultiSlotTeam.Red;
                    break;
                case MultiSlotTeam.Red:
                    slot.Team = MultiSlotTeam.Blue;
                    break;
                case MultiSlotTeam.NoTeam:
                    slot.Team = new Random().Next(1) == 1 ? MultiSlotTeam.Red : MultiSlotTeam.Blue;
                    break;
                default:
                    slot.Team = MultiSlotTeam.NoTeam;
                    break;
            }

            LPacketStreams.GetStream("lobby").Broadcast(new MatchUpdate(pr.JoinedRoom));
            pr.JoinedRoom.Broadcast(new MatchUpdate(pr.JoinedRoom));
        }

        [Handler(HandlerTypes.ClientMatchReady)]
        public void OnMatchReady(Presence pr)
        {
            MultiplayerSlot slot = pr.JoinedRoom?.Slots.First(x => pr.User.Id == x.UserId);
            if (slot == null) return;

            slot.Status = MultiSlotStatus.Ready;

            LPacketStreams.GetStream("lobby").Broadcast(new MatchUpdate(pr.JoinedRoom));
            pr.JoinedRoom.Broadcast(new MatchUpdate(pr.JoinedRoom));
        }

        [Handler(HandlerTypes.ClientMatchNotReady)]
        public void OnMatchNotReady(Presence pr)
        {
            MultiplayerSlot slot = pr.JoinedRoom?.Slots.First(x => pr.User.Id == x.UserId);
            if (slot == null) return;

            slot.Status = MultiSlotStatus.NotReady;

            LPacketStreams.GetStream("lobby").Broadcast(new MatchUpdate(pr.JoinedRoom));
            pr.JoinedRoom.Broadcast(new MatchUpdate(pr.JoinedRoom));
        }

        [Handler(HandlerTypes.ClientMatchTransferHost)]
        public void OnMatchTransferHost(Presence pr, int slotId)
        {
            if (pr.JoinedRoom == null || pr.JoinedRoom.HostId != pr.User.Id)
                return;
            if (slotId > 16) return;
            MultiplayerSlot slot = pr.JoinedRoom.Slots[slotId];

            pr.JoinedRoom.HostId = slot.UserId;
            
            LPacketStreams.GetStream("lobby").Broadcast(new MatchUpdate(pr.JoinedRoom));
            pr.JoinedRoom.Broadcast(new MatchUpdate(pr.JoinedRoom));
        }

        [Handler(HandlerTypes.ClientMatchNoBeatmap)]
        public void OnMatchNoBeatmap(Presence pr)
        {
            if (pr.JoinedRoom == null)
                return;
            MultiplayerSlot slot = pr.JoinedRoom.Slots.First(x => x.UserId == pr.User.Id);

            slot.Status = MultiSlotStatus.NoMap;

            LPacketStreams.GetStream("lobby").Broadcast(new MatchUpdate(pr.JoinedRoom));
            pr.JoinedRoom.Broadcast(new MatchUpdate(pr.JoinedRoom));
        }

        [Handler(HandlerTypes.ClientMatchHasBeatmap)]
        public void OnMatchHasBeatmap(Presence pr)
        {
            if (pr.JoinedRoom == null)
                return;
            MultiplayerSlot slot = pr.JoinedRoom.Slots.First(x => x.UserId == pr.User.Id);

            slot.Status = MultiSlotStatus.NotReady;

            LPacketStreams.GetStream("lobby").Broadcast(new MatchUpdate(pr.JoinedRoom));
            pr.JoinedRoom.Broadcast(new MatchUpdate(pr.JoinedRoom));
        }

        [Handler(HandlerTypes.ClientInvite)]
        public void OnInvite(Presence pr, int userId)
        {
            if (pr.JoinedRoom == null) return;
            // Took me a while to figure out. i tried osu://mp/matchid/Password.Replace(" ", "_");
            string inviteUri = $"osump://{pr.JoinedRoom.MatchId}/{pr.JoinedRoom.Password.Replace(" ", "_")}";

            Presence opr = LPresences.GetPresence(userId);
            opr.Write(new Invite(new MessageStruct
            {
                ChannelTarget = pr.User.Username,
                Message = $"\0Hey, I want to play with you! Join me [{inviteUri} {pr.JoinedRoom.Name}]",
                Username = pr.User.Username,
                SenderId = pr.User.Id
            }));
        }

        [Handler(HandlerTypes.ClientMatchStart)]
        public void OnMatchStart(Presence pr)
        {
            if (pr.JoinedRoom == null) return;
            if (pr.JoinedRoom.HostId != pr.User.Id) return;

            pr.JoinedRoom.InProgress = true;

            foreach (MultiplayerSlot slot in pr.JoinedRoom.Slots.Where(x => x.UserId != -1 && x.Status != MultiSlotStatus.NoMap)) {
                slot.Status = MultiSlotStatus.Playing;
                ++pr.JoinedRoom.NeedLoad;
            }
            
            pr.JoinedRoom.Broadcast(new MatchStart(pr.JoinedRoom));
            LPacketStreams.GetStream("lobby").Broadcast(new MatchUpdate(pr.JoinedRoom));
            pr.JoinedRoom.Broadcast(new MatchUpdate(pr.JoinedRoom));
        }

        [Handler(HandlerTypes.ClientMatchLoadComplete)]
        public void OnMatchLoadComplete(Presence pr)
        {
            MultiplayerSlot slot = pr.JoinedRoom?.Slots.FirstOrDefault(x => x.UserId == pr.User.Id);
            if (slot == null) return;
            
            if (--pr.JoinedRoom.NeedLoad == 0)
                pr.JoinedRoom.Broadcast(new MatchAllPlayersLoaded());
            
            LPacketStreams.GetStream("lobby").Broadcast(new MatchUpdate(pr.JoinedRoom));
            pr.JoinedRoom.Broadcast(new MatchUpdate(pr.JoinedRoom));
        }
        #endregion
    }
}