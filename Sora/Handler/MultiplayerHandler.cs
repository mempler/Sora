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

namespace Sora.Handler
{
    using System;
    using System.Collections.Generic;
    using Enums;
    using Objects;
    using Packets.Client;
    using Packets.Server;
    using Shared.Enums;
    using Shared.Handlers;
    using MatchScoreUpdate = Packets.Server.MatchScoreUpdate;

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
        public void OnLobbyLeft(Presence pr) => LPacketStreams.GetStream("lobby").Left(pr);

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

            room.Update();
        }

        [Handler(HandlerTypes.ClientMatchPart)]
        public void OnMatchLeave(Presence pr)
        {
            if (pr.JoinedRoom == null) return;

            MultiplayerRoom room = pr.JoinedRoom;
            room.Leave(pr);
            if (room.HostId == pr.User.Id)
                room.SetRandomHost();

            if (room.HostId != -1)
            {
                room.Update();
                return;
            }

            room.Dispand();
        }

        [Handler(HandlerTypes.ClientMatchJoin)]
        public void OnMatchJoin(Presence pr, int matchId, string password)
        {
            MultiplayerRoom room = MultiplayerLobby.GetRoom(matchId);
            if (room != null && room.Join(pr, password.Replace(" ", "_")))
                pr.Write(new MatchJoinSuccess(room));
            else
                pr.Write(new MatchJoinFail());

            room?.Update();
        }

        [Handler(HandlerTypes.ClientMatchChangeSlot)]
        public void OnMatchChangeSlot(Presence pr, int slotId)
        {
            if (pr.JoinedRoom == null) return;
            if (slotId > 16) return;
            MultiplayerSlot newSlot = pr.JoinedRoom.Slots[slotId];
            if (newSlot.UserId != -1) return;

            MultiplayerSlot oldSlot = pr.JoinedRoom.GetSlotByUserId(pr.User.Id);

            pr.JoinedRoom.SetSlot(newSlot, oldSlot);
            pr.JoinedRoom.ClearSlot(oldSlot);
        }

        [Handler(HandlerTypes.ClientMatchChangeMods)]
        public void OnMatchChangeMods(Presence pr, Mod mods)
        {
            MultiplayerSlot slot = pr.JoinedRoom?.GetSlotByUserId(pr.User.Id);
            if (slot == null) return;
            pr.JoinedRoom.SetMods(mods, slot);
        }

        [Handler(HandlerTypes.ClientMatchChangeSettings)]
        public void OnMatchChangeSettings(Presence pr, MultiplayerRoom room)
        {
            if (pr.JoinedRoom == null) return;
            if (pr.JoinedRoom.HostId != pr.User.Id) return;

            pr.JoinedRoom.ChangeSettings(room);
        }

        [Handler(HandlerTypes.ClientMatchChangePassword)]
        public void OnMatchChangePassword(Presence pr, MultiplayerRoom room)
        {
            if (pr.JoinedRoom == null) return;
            if (pr.JoinedRoom.HostId != pr.User.Id) return;

            pr.JoinedRoom.SetPassword(room.Password);
        }

        [Handler(HandlerTypes.ClientMatchLock)]
        public void OnMatchLock(Presence pr, int slotId)
        {
            if (pr.JoinedRoom == null || pr.JoinedRoom.HostId != pr.User.Id) return;
            if (slotId > 16) return;

            pr.JoinedRoom.LockSlot(pr.JoinedRoom.Slots[slotId]);
        }

        [Handler(HandlerTypes.ClientMatchChangeTeam)]
        public void OnMatchChangeTeam(Presence pr)
        {
            MultiplayerSlot slot = pr.JoinedRoom?.GetSlotByUserId(pr.User.Id);
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

            pr.JoinedRoom.Update();
        }

        [Handler(HandlerTypes.ClientMatchReady)]
        public void OnMatchReady(Presence pr)
        {
            MultiplayerSlot slot = pr.JoinedRoom?.GetSlotByUserId(pr.User.Id);
            if (slot == null) return;

            slot.Status = MultiSlotStatus.Ready;

            pr.JoinedRoom.Update();
        }

        [Handler(HandlerTypes.ClientMatchNotReady)]
        public void OnMatchNotReady(Presence pr)
        {
            MultiplayerSlot slot = pr.JoinedRoom?.GetSlotByUserId(pr.User.Id);
            if (slot == null) return;

            slot.Status = MultiSlotStatus.NotReady;

            pr.JoinedRoom.Update();
        }

        [Handler(HandlerTypes.ClientMatchTransferHost)]
        public void OnMatchTransferHost(Presence pr, int slotId)
        {
            if (pr.JoinedRoom == null || pr.JoinedRoom.HostId != pr.User.Id)
                return;
            if (slotId > 16) return;
            MultiplayerSlot slot = pr.JoinedRoom.Slots[slotId];

            pr.JoinedRoom.SetHost(slot.UserId);
        }

        [Handler(HandlerTypes.ClientMatchNoBeatmap)]
        public void OnMatchNoBeatmap(Presence pr)
        {
            if (pr.JoinedRoom == null)
                return;
            MultiplayerSlot slot = pr.JoinedRoom.GetSlotByUserId(pr.User.Id);

            slot.Status = MultiSlotStatus.NoMap;

            pr.JoinedRoom.Update();
        }

        [Handler(HandlerTypes.ClientMatchHasBeatmap)]
        public void OnMatchHasBeatmap(Presence pr)
        {
            if (pr.JoinedRoom == null)
                return;
            MultiplayerSlot slot = pr.JoinedRoom.GetSlotByUserId(pr.User.Id);

            slot.Status = MultiSlotStatus.NotReady;

            pr.JoinedRoom.Update();
        }

        [Handler(HandlerTypes.ClientInvite)]
        public void OnInvite(Presence pr, int userId)
        {
            if (pr.JoinedRoom == null) return;
            Presence opr = LPresences.GetPresence(userId);
            if (opr == null) return;
            pr.JoinedRoom.Invite(opr);
        }

        [Handler(HandlerTypes.ClientMatchStart)]
        public void OnMatchStart(Presence pr)
        {
            if (pr.JoinedRoom == null || pr.JoinedRoom.HostId != pr.User.Id) return;
            pr.JoinedRoom.Start();
        }

        [Handler(HandlerTypes.ClientMatchLoadComplete)]
        public void OnMatchLoadComplete(Presence pr)
        {
            if (pr.JoinedRoom?.GetSlotByUserId(pr.User.Id) != null)
                pr.JoinedRoom.LoadComplete();
        }

        [Handler(HandlerTypes.ClientMatchScoreUpdate)]
        public void OnMatchScoreUpdate(Presence pr, ScoreFrame frame)
            => pr.JoinedRoom?.Broadcast(new MatchScoreUpdate(pr.JoinedRoom.GetSlotIdByUserId(pr.User.Id), frame));

        [Handler(HandlerTypes.ClientMatchFailed)]
        public void OnMatchFailed(Presence pr) => pr.JoinedRoom?.Failed(pr);

        [Handler(HandlerTypes.ClientMatchComplete)]
        public void OnMatchComplete(Presence pr) => pr.JoinedRoom?.Complete(pr);

        [Handler(HandlerTypes.ClientMatchSkipRequest)]
        public void OnMatchSkipRequest(Presence pr) => pr.JoinedRoom?.Skip(pr);

    #endregion
    }
}
