using EventManager.Attributes;
using EventManager.Enums;
using Shared.Enums;
using Shared.Helpers;
using Sora.EventArgs;
using Sora.Objects;
using Sora.Packets.Client;
using Sora.Packets.Server;
using Sora.Services;
using Invite = Sora.Packets.Client.Invite;
using MatchScoreUpdate = Sora.Packets.Client.MatchScoreUpdate;
using SendIrcMessage = Sora.Packets.Client.SendIrcMessage;
using SpectatorFrames = Sora.Packets.Client.SpectatorFrames;

namespace Sora.Events
{
    [EventClass]
    public class OnBanchoPacketEvent
    {
        private readonly EventManager.EventManager _evmgr;
        private readonly PacketStreamService _pss;
        private readonly MultiplayerService _ms;
        private readonly PresenceService _ps;

        public OnBanchoPacketEvent(EventManager.EventManager evmgr,
                                   PacketStreamService pss,
                                   MultiplayerService ms,
                                   PresenceService ps)
        {
            _evmgr = evmgr;
            _pss = pss;
            _ms = ms;
            _ps = ps;
        }
        
        [Event(EventType.BanchoPacket)]
        public void OnBanchoPacket(BanchoPacketArgs args)
        {
            switch (args.PacketId)
            {
                #region Stats
                case PacketId.ClientSendUserStatus:
                    _evmgr.RunEvent(
                        EventType.BanchoSendUserStatus,
                        new BanchoSendUserStatusArgs
                            {pr = args.pr, status = args.Data.ReadData<SendUserStatus>().Status});
                    break;
                case PacketId.ClientPong:
                    _evmgr.RunEvent(
                        EventType.BanchoPong,
                        new BanchoPongArgs()
                            {pr = args.pr});
                    break;
                case PacketId.ClientRequestStatusUpdate:
                    _evmgr.RunEvent(
                        EventType.BanchoRequestStatusUpdate,
                        new BanchoSendUserStatusArgs()
                            {pr = args.pr, status = args.pr.Status });
                    break;
                case PacketId.ClientUserStatsRequest:
                    _evmgr.RunEvent(
                        EventType.BanchoUserStatsRequest,
                        new BanchoUserStatsRequestArgs()
                            {pr = args.pr, userIds = args.Data.ReadData<UserStatsRequest>().Userids });
                    break;
                /*
                case PacketId.ClientReceiveUpdates:
                    _evmgr.RunEvent(
                        EventType.BanchoReceiveUpdates);
                    break;
                */
                #endregion
                
                #region Channels

                case PacketId.ClientChannelJoin:
                    _evmgr.RunEvent(
                        EventType.BanchoChannelJoin,
                        new BanchoChannelJoinArgs
                            {pr = args.pr, ChannelName = args.Data.ReadData<ChannelJoin>().ChannelName});
                    break;
                case PacketId.ClientChannelLeave:
                    _evmgr.RunEvent(
                        EventType.BanchoChannelLeave,
                        new BanchoChannelLeaveArgs
                            {pr = args.pr, ChannelName = args.Data.ReadData<ChannelLeave>().ChannelName});
                    break;

                #endregion

                #region Chat

                case PacketId.ClientSendIrcMessagePrivate:
                case PacketId.ClientSendIrcMessage:                   
                    SendIrcMessage msg = args.Data.ReadData<SendIrcMessage>();
                    _evmgr.RunEvent(
                        msg.Msg.ChannelTarget.StartsWith("#")
                            ? EventType.BanchoSendIrcMessage
                            : EventType.BanchoSendIrcMessagePrivate,
                        new BanchoSendIRCMessageArgs()
                            {pr = args.pr, Message = msg.Msg});

                    break;

                #endregion

                #region Friends

                case PacketId.ClientFriendAdd:
                    _evmgr.RunEvent(
                        EventType.BanchoFriendAdd,
                        new BanchoFriendAddArgs()
                            {pr = args.pr, FriendId = args.Data.ReadData<FriendAdd>().FriendId});
                    break;
                case PacketId.ClientFriendRemove:
                    _evmgr.RunEvent(
                        EventType.BanchoFriendRemove,
                        new BanchoFriendRemoveArgs()
                            {pr = args.pr, FriendId = args.Data.ReadData<FriendRemove>().FriendId});
                    break;

                #endregion

                #region Spectator

                case PacketId.ClientStartSpectating:
                    _evmgr.RunEvent(
                        EventType.BanchoStartSpectating,
                        new BanchoStartSpectatingArgs()
                            {pr = args.pr, SpectatorHostID = args.Data.ReadData<StartSpectating>().ToSpectateId});
                    break;
                case PacketId.ClientStopSpectating:
                    _evmgr.RunEvent(
                        EventType.BanchoStopSpectating,
                        new BanchoStopSpectatingArgs()
                            {pr = args.pr});
                    break;
                case PacketId.ClientCantSpectate:
                    _evmgr.RunEvent(
                        EventType.BanchoCantSpectate,
                        new BanchoCantSpectateArgs()
                            {pr = args.pr});
                    break;
                case PacketId.ClientSpectateFrames:
                    _evmgr.RunEvent(
                        EventType.BanchoBroadcastFrames,
                        new BanchoBroadcastFramesArgs()
                            {pr = args.pr, frames = args.Data.ReadData<SpectatorFrames>().Frames});
                    break;

                #endregion

                #region Multi

                case PacketId.ClientLobbyJoin:
                    _evmgr.RunEvent(
                        EventType.BanchoLobbyJoin,
                        new BanchoLobbyJoinArgs()
                            {pr = args.pr});
                    break;
                case PacketId.ClientLobbyPart:
                    _evmgr.RunEvent(
                        EventType.BanchoLobbyPart,
                        new BanchoLobbyPartArgs()
                            {pr = args.pr});
                    break;
                case PacketId.ClientMatchCreate:
                    MultiplayerRoom x = args.Data.ReadData<MatchCreate>().Room;
                    x._pss = _pss;
                    x._ms = _ms;
                    x._ps = _ps;
                    
                    _evmgr.RunEvent(
                        EventType.BanchoMatchCreate,
                        new BanchoMatchCreateArgs()
                            { pr = args.pr, room = x });
                    break;
                case PacketId.ClientMatchJoin:
                    MatchJoin mJoin = args.Data.ReadData<MatchJoin>();
                    
                    _evmgr.RunEvent(
                        EventType.BanchoMatchJoin,
                        new BanchoMatchJoinArgs()
                            {pr = args.pr, matchId = mJoin.MatchId, password = mJoin.Password});
                    break;
                case PacketId.ClientMatchPart:
                    _evmgr.RunEvent(
                        EventType.BanchoMatchPart,
                        new BanchoMatchPartArgs()
                            {pr = args.pr});
                    break;
                case PacketId.ClientMatchChangeSlot:
                    _evmgr.RunEvent(
                        EventType.BanchoMatchChangeSlot,
                        new BanchoMatchChangeSlotArgs()
                            {pr = args.pr, SlotId = args.Data.ReadData<MatchChangeSlot>().SlotId});
                    break;
                case PacketId.ClientMatchChangeMods:
                    _evmgr.RunEvent(
                        EventType.BanchoMatchChangeMods,
                        new BanchoMatchChangeModsArgs()
                            {pr = args.pr, mods = args.Data.ReadData<MatchChangeMods>().Mods});
                    break;
                case PacketId.ClientMatchLock:
                    _evmgr.RunEvent(
                        EventType.BanchoMatchChangeMods,
                        new BanchoMatchLockArgs()
                            {pr = args.pr, SlotId = args.Data.ReadData<MatchLock>().SlotId});
                    break;
                case PacketId.ClientMatchChangeSettings:
                    MultiplayerRoom y = args.Data.ReadData<MatchChangeSettings>().Room;
                    y._pss = _pss;
                    y._ms  = _ms;
                    y._ps  = _ps;
                    
                    _evmgr.RunEvent(
                        EventType.BanchoMatchChangeSettings,
                        new BanchoMatchChangeSettingsArgs()
                            {pr = args.pr, room = y });
                    break;
                case PacketId.ClientMatchChangePassword:
                    MultiplayerRoom xy = args.Data.ReadData<MatchChangePassword>().Room;
                    xy._pss = _pss;
                    xy._ms  = _ms;
                    xy._ps  = _ps;

                    _evmgr.RunEvent(
                        EventType.BanchoMatchChangeSettings,
                        new BanchoMatchChangePasswordArgs()
                            {pr = args.pr, room = xy});
                    break;
                case PacketId.ClientMatchChangeTeam:
                    _evmgr.RunEvent(
                        EventType.BanchoMatchChangeTeam,
                        new BanchoMatchChangeTeamArgs()
                            {pr = args.pr});
                    break;
                case PacketId.ClientMatchReady:
                    _evmgr.RunEvent(
                        EventType.BanchoMatchReady,
                        new BanchoMatchReadyArgs()
                            {pr = args.pr});
                    break;
                case PacketId.ClientMatchNotReady:
                    _evmgr.RunEvent(
                        EventType.BanchoMatchNotReady,
                        new BanchoMatchNotReadyArgs()
                            {pr = args.pr});
                    break;
                case PacketId.ClientMatchTransferHost:
                    _evmgr.RunEvent(
                        EventType.BanchoMatchTransferHost,
                        new BanchoMatchTransferHostArgs()
                            {pr = args.pr, SlotId = args.Data.ReadData<MatchTransferHost>().SlotId});
                    break;
                case PacketId.ClientMatchNoBeatmap:
                    _evmgr.RunEvent(
                        EventType.BanchoMatchNoBeatmap,
                        new BanchoMatchNoBeatmapArgs()
                            {pr = args.pr});
                    break;
                case PacketId.ClientMatchHasBeatmap:
                    _evmgr.RunEvent(
                        EventType.BanchoMatchHasBeatmap,
                        new BanchoMatchHasBeatmapArgs()
                            {pr = args.pr});
                    break;
                case PacketId.ClientInvite:
                    _evmgr.RunEvent(
                        EventType.BanchoInvite,
                        new BanchoInviteArgs()
                            {pr = args.pr, UserId = args.Data.ReadData<Invite>().UserId});
                    break;
                case PacketId.ClientMatchStart:
                    _evmgr.RunEvent(
                        EventType.BanchoMatchStart,
                        new BanchoMatchStartArgs()
                            {pr = args.pr});
                    break;
                case PacketId.ClientMatchLoadComplete:
                    _evmgr.RunEvent(
                        EventType.BanchoMatchLoadComplete,
                        new BanchoMatchLoadCompleteArgs()
                            {pr = args.pr});
                    break;
                case PacketId.ClientMatchScoreUpdate:
                    _evmgr.RunEvent(
                        EventType.BanchoMatchScoreUpdate,
                        new BanchoMatchScoreUpdateArgs()
                            {pr = args.pr, Frame = args.Data.ReadData<MatchScoreUpdate>().Frame});
                    break;
                case PacketId.ClientMatchFailed:
                    _evmgr.RunEvent(
                        EventType.BanchoMatchFailed,
                        new BanchoMatchFailedArgs()
                            {pr = args.pr});
                    break;
                case PacketId.ClientMatchSkipRequest:
                    _evmgr.RunEvent(
                        EventType.BanchoMatchSkipRequest,
                        new BanchoMatchSkipRequestArgs()
                            {pr = args.pr});
                    break;
                case PacketId.ClientMatchComplete:
                    _evmgr.RunEvent(
                        EventType.BanchoMatchComplete,
                        new BanchoMatchCompleteArgs()
                            {pr = args.pr});
                    break;

                #endregion

                #region Other

                case PacketId.ClientExit:
                    _evmgr.RunEvent(
                        EventType.BanchoExit,
                        new BanchoExitArgs()
                            {pr = args.pr, err = args.Data.ReadData<Exit>().ErrorState});
                    break;
                default:
                    Logger.Debug(
                        $"PacketId: {args.PacketId} Length: {args.Data.BaseStream.Length} Data: {Hex.ToHex(args.Data.ReadToEnd())}");
                    break;

                #endregion
            }
        }
    }
}