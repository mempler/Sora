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
using System.Threading.Tasks;
using Sora.Attributes;
using Sora.Enums;
using Sora.EventArgs;
using Sora.Helpers;
using Sora.Interfaces;
using Sora.Packets.Client;
using Sora.Services;

namespace Sora.Events.BanchoEvents
{
    [EventClass]
    public class OnBanchoPacketEvent
    {
        private readonly EventManager _evmgr;
        private readonly MultiplayerService _ms;
        private readonly PresenceService _ps;
        private readonly PacketStreamService _pss;

        public OnBanchoPacketEvent(EventManager evmgr,
            PacketStreamService pss,
            MultiplayerService ms,
            PresenceService ps)
        {
            _evmgr = evmgr;
            _pss = pss;
            _ms = ms;
            _ps = ps;
        }

        /*
        private object GetRightEventArgs(PacketId id)
        {
            IPacket p = new EmptyPacket();
            IEnumerable<Type> packets = 
                AppDomain.CurrentDomain
                     .GetAssemblies()
                     .SelectMany(t => t.GetTypes())
                     .Where(t => t.IsClass && t.IsAssignableFrom(typeof(IPacket)))
                     .Where(t =>
                     {
                         Logger.Info("Prop", t.GetProperty("Id"));
                         return (PacketId) (t.GetProperty("Id")?.GetValue(p) ?? -1) == id;
                     });

            IEnumerable<object> inPack = packets.Select(Activator.CreateInstance);
            return inPack.FirstOrDefault();
        }
        */

        [Event(EventType.BanchoPacket)]
        public async Task OnBanchoPacket(BanchoPacketArgs args)
        {
            switch (args.PacketId)
            {
                #region Stats

                case PacketId.ClientSendUserStatus:
                    await _evmgr.RunEvent(
                        EventType.BanchoSendUserStatus,
                        new BanchoSendUserStatusArgs
                        {
                            pr = args.pr, status = args.Data.ReadData<SendUserStatus>().Status
                        }
                    );
                    break;
                case PacketId.ClientPong:
                    await _evmgr.RunEvent(
                        EventType.BanchoPong,
                        new BanchoPongArgs {pr = args.pr}
                    );
                    break;
                case PacketId.ClientRequestStatusUpdate:
                    await _evmgr.RunEvent(
                        EventType.BanchoRequestStatusUpdate,
                        new BanchoSendUserStatusArgs {pr = args.pr, status = args.pr.Get<UserStatus>("STATUS")}
                    );
                    break;
                case PacketId.ClientUserStatsRequest:
                    await _evmgr.RunEvent(
                        EventType.BanchoUserStatsRequest,
                        new BanchoUserStatsRequestArgs
                        {
                            pr = args.pr, userIds = args.Data.ReadData<UserStatsRequest>().Userids
                        }
                    );
                    break;
                case PacketId.ClientReceiveUpdates:
                    await _evmgr.RunEvent(
                        EventType.BanchoReceiveUpdates,
                        new BanchoEmptyEventArgs {pr = args.pr}
                    );
                    break;
                case PacketId.ClientUserPresenceRequest:
                    await _evmgr.RunEvent(
                        EventType.BanchoUserPresenceRequest,
                        new BanchoClientUserPresenceRequestArgs {pr = args.pr, userIds = args.Data.ReadData<UserPresenceRequest>().UserIds }
                    );
                    break;

                #endregion

                #region Channels

                case PacketId.ClientChannelJoin:
                    await _evmgr.RunEvent(
                        EventType.BanchoChannelJoin,
                        new BanchoChannelJoinArgs
                        {
                            pr = args.pr, ChannelName = args.Data.ReadData<ChannelJoin>().ChannelName
                        }
                    );
                    break;
                case PacketId.ClientChannelLeave:
                    await _evmgr.RunEvent(
                        EventType.BanchoChannelLeave,
                        new BanchoChannelLeaveArgs
                        {
                            pr = args.pr, ChannelName = args.Data.ReadData<ChannelLeave>().ChannelName
                        }
                    );
                    break;

                #endregion

                #region Chat

                case PacketId.ClientSendIrcMessagePrivate:
                case PacketId.ClientSendIrcMessage:
                    var msg = args.Data.ReadData<SendIrcMessage>();
                    await _evmgr.RunEvent(
                        msg.Msg.ChannelTarget.StartsWith("#")
                            ? EventType.BanchoSendIrcMessage
                            : EventType.BanchoSendIrcMessagePrivate,
                        new BanchoSendIRCMessageArgs {pr = args.pr, Message = msg.Msg}
                    );

                    break;

                #endregion

                #region Friends

                case PacketId.ClientFriendAdd:
                    await _evmgr.RunEvent(
                        EventType.BanchoFriendAdd,
                        new BanchoFriendAddArgs {pr = args.pr, FriendId = args.Data.ReadData<FriendAdd>().FriendId}
                    );
                    break;
                case PacketId.ClientFriendRemove:
                    await _evmgr.RunEvent(
                        EventType.BanchoFriendRemove,
                        new BanchoFriendRemoveArgs
                        {
                            pr = args.pr, FriendId = args.Data.ReadData<FriendRemove>().FriendId
                        }
                    );
                    break;

                #endregion

                #region Spectator

                case PacketId.ClientStartSpectating:
                    await _evmgr.RunEvent(
                        EventType.BanchoStartSpectating,
                        new BanchoStartSpectatingArgs
                        {
                            pr = args.pr, SpectatorHostID = args.Data.ReadData<StartSpectating>().ToSpectateId
                        }
                    );
                    break;
                case PacketId.ClientStopSpectating:
                    await _evmgr.RunEvent(
                        EventType.BanchoStopSpectating,
                        new BanchoStopSpectatingArgs {pr = args.pr}
                    );
                    break;
                case PacketId.ClientCantSpectate:
                    await _evmgr.RunEvent(
                        EventType.BanchoCantSpectate,
                        new BanchoCantSpectateArgs {pr = args.pr}
                    );
                    break;
                case PacketId.ClientSpectateFrames:
                    await _evmgr.RunEvent(
                        EventType.BanchoBroadcastFrames,
                        new BanchoBroadcastFramesArgs
                        {
                            pr = args.pr, frames = args.Data.ReadData<SpectatorFrames>().Frames
                        }
                    );
                    break;

                #endregion

                #region Multi

                case PacketId.ClientLobbyJoin:
                    await _evmgr.RunEvent(
                        EventType.BanchoLobbyJoin,
                        new BanchoLobbyJoinArgs {pr = args.pr}
                    );
                    break;
                case PacketId.ClientLobbyPart:
                    await _evmgr.RunEvent(
                        EventType.BanchoLobbyPart,
                        new BanchoLobbyPartArgs {pr = args.pr}
                    );
                    break;
                case PacketId.ClientMatchCreate:
                    var x = args.Data.ReadData<MatchCreate>().Room;
                    x._pss = _pss;
                    x._ms = _ms;
                    x._ps = _ps;

                    await _evmgr.RunEvent(
                        EventType.BanchoMatchCreate,
                        new BanchoMatchCreateArgs {pr = args.pr, room = x}
                    );
                    break;
                case PacketId.ClientMatchJoin:
                    var mJoin = args.Data.ReadData<MatchJoin>();

                    await _evmgr.RunEvent(
                        EventType.BanchoMatchJoin,
                        new BanchoMatchJoinArgs {pr = args.pr, matchId = mJoin.MatchId, password = mJoin.Password}
                    );
                    break;
                case PacketId.ClientMatchPart:
                    await _evmgr.RunEvent(
                        EventType.BanchoMatchPart,
                        new BanchoMatchPartArgs {pr = args.pr}
                    );
                    break;
                case PacketId.ClientMatchChangeSlot:
                    await _evmgr.RunEvent(
                        EventType.BanchoMatchChangeSlot,
                        new BanchoMatchChangeSlotArgs
                        {
                            pr = args.pr, SlotId = args.Data.ReadData<MatchChangeSlot>().SlotId
                        }
                    );
                    break;
                case PacketId.ClientMatchChangeMods:
                    await _evmgr.RunEvent(
                        EventType.BanchoMatchChangeMods,
                        new BanchoMatchChangeModsArgs {pr = args.pr, mods = args.Data.ReadData<MatchChangeMods>().Mods}
                    );
                    break;
                case PacketId.ClientMatchLock:
                    await _evmgr.RunEvent(
                        EventType.BanchoMatchLock,
                        new BanchoMatchLockArgs {pr = args.pr, SlotId = args.Data.ReadData<MatchLock>().SlotId}
                    );
                    break;
                case PacketId.ClientMatchChangeSettings:
                    var y = args.Data.ReadData<MatchChangeSettings>().Room;
                    y._pss = _pss;
                    y._ms = _ms;
                    y._ps = _ps;

                    await _evmgr.RunEvent(
                        EventType.BanchoMatchChangeSettings,
                        new BanchoMatchChangeSettingsArgs {pr = args.pr, room = y}
                    );
                    break;
                case PacketId.ClientMatchChangePassword:
                    var xy = args.Data.ReadData<MatchChangePassword>().Room;
                    xy._pss = _pss;
                    xy._ms = _ms;
                    xy._ps = _ps;

                    await _evmgr.RunEvent(
                        EventType.BanchoMatchChangePassword,
                        new BanchoMatchChangePasswordArgs {pr = args.pr, room = xy}
                    );
                    break;
                case PacketId.ClientMatchChangeTeam:
                    await _evmgr.RunEvent(
                        EventType.BanchoMatchChangeTeam,
                        new BanchoMatchChangeTeamArgs {pr = args.pr}
                    );
                    break;
                case PacketId.ClientMatchReady:
                    await _evmgr.RunEvent(
                        EventType.BanchoMatchReady,
                        new BanchoMatchReadyArgs {pr = args.pr}
                    );
                    break;
                case PacketId.ClientMatchNotReady:
                    await _evmgr.RunEvent(
                        EventType.BanchoMatchNotReady,
                        new BanchoMatchNotReadyArgs {pr = args.pr}
                    );
                    break;
                case PacketId.ClientMatchTransferHost:
                    await _evmgr.RunEvent(
                        EventType.BanchoMatchTransferHost,
                        new BanchoMatchTransferHostArgs
                        {
                            pr = args.pr, SlotId = args.Data.ReadData<MatchTransferHost>().SlotId
                        }
                    );
                    break;
                case PacketId.ClientMatchNoBeatmap:
                    await _evmgr.RunEvent(
                        EventType.BanchoMatchNoBeatmap,
                        new BanchoMatchNoBeatmapArgs {pr = args.pr}
                    );
                    break;
                case PacketId.ClientMatchHasBeatmap:
                    await _evmgr.RunEvent(
                        EventType.BanchoMatchHasBeatmap,
                        new BanchoMatchHasBeatmapArgs {pr = args.pr}
                    );
                    break;
                case PacketId.ClientInvite:
                    await _evmgr.RunEvent(
                        EventType.BanchoInvite,
                        new BanchoInviteArgs {pr = args.pr, UserId = args.Data.ReadData<Invite>().UserId}
                    );
                    break;
                case PacketId.ClientMatchStart:
                    await _evmgr.RunEvent(
                        EventType.BanchoMatchStart,
                        new BanchoMatchStartArgs {pr = args.pr}
                    );
                    break;
                case PacketId.ClientMatchLoadComplete:
                    await _evmgr.RunEvent(
                        EventType.BanchoMatchLoadComplete,
                        new BanchoMatchLoadCompleteArgs {pr = args.pr}
                    );
                    break;
                case PacketId.ClientMatchScoreUpdate:
                    await _evmgr.RunEvent(
                        EventType.BanchoMatchScoreUpdate,
                        new BanchoMatchScoreUpdateArgs
                        {
                            pr = args.pr, Frame = args.Data.ReadData<MatchScoreUpdate>().Frame
                        }
                    );
                    break;
                case PacketId.ClientMatchFailed:
                    await _evmgr.RunEvent(
                        EventType.BanchoMatchFailed,
                        new BanchoMatchFailedArgs {pr = args.pr}
                    );
                    break;
                case PacketId.ClientMatchSkipRequest:
                    await _evmgr.RunEvent(
                        EventType.BanchoMatchSkipRequest,
                        new BanchoMatchSkipRequestArgs {pr = args.pr}
                    );
                    break;
                case PacketId.ClientMatchComplete:
                    await _evmgr.RunEvent(
                        EventType.BanchoMatchComplete,
                        new BanchoMatchCompleteArgs {pr = args.pr}
                    );
                    break;

                #endregion

                #region Other

                case PacketId.ClientExit:
                    await _evmgr.RunEvent(
                        EventType.BanchoExit,
                        new BanchoExitArgs {pr = args.pr, err = args.Data.ReadData<Exit>().ErrorState}
                    );
                    break;
                default:
                    Logger.Debug(
                        $"PacketId: {args.PacketId} Length: {args.Data.BaseStream.Length} Data: {Hex.ToHex(args.Data.ReadToEnd())}"
                    );
                    break;

                #endregion
            }
        }

        private class EmptyPacket : IPacket
        {
            public void ReadFromStream(MStreamReader sr)
            {
                throw new NotImplementedException();
            }

            public void WriteToStream(MStreamWriter sw)
            {
                throw new NotImplementedException();
            }

            public PacketId Id { get; }
        }
    }
}
