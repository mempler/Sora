using System.Threading.Tasks;
using Sora.Attributes;
using Sora.Enums;
using Sora.EventArgs.BanchoEventArgs;
using Sora.Framework.Enums;
using Sora.Framework.Packets.Client;
using Sora.Framework.Utilities;

namespace Sora.Events.BanchoEvents
{
    [EventClass]
    public class OnBanchoPacketEvent
    {
        private readonly EventManager _evmgr;

        public OnBanchoPacketEvent(EventManager evmgr)
        {
            _evmgr = evmgr;
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
                            Pr = args.Pr, Status = args.Data.ReadData<SendUserStatus>().Status
                        }
                    );
                    break;
                case PacketId.ClientPong:
                    await _evmgr.RunEvent(
                        EventType.BanchoPong,
                        new BanchoPongArgs {Pr = args.Pr}
                    );
                    break;
                case PacketId.ClientRequestStatusUpdate:
                    await _evmgr.RunEvent(
                        EventType.BanchoRequestStatusUpdate,
                        new BanchoSendUserStatusArgs {Pr = args.Pr, Status = args.Pr.Status}
                    );
                    break;
                case PacketId.ClientUserStatsRequest:
                    await _evmgr.RunEvent(
                        EventType.BanchoUserStatsRequest,
                        new BanchoUserStatsRequestArgs
                        {
                            Pr = args.Pr, UserIds = args.Data.ReadData<UserStatsRequest>().Userids
                        }
                    );
                    break;
                case PacketId.ClientReceiveUpdates:
                    await _evmgr.RunEvent(
                        EventType.BanchoReceiveUpdates,
                        new BanchoEmptyEventArgs {Pr = args.Pr}
                    );
                    break;
                case PacketId.ClientUserPresenceRequest:
                    await _evmgr.RunEvent(
                        EventType.BanchoUserPresenceRequest,
                        new BanchoClientUserPresenceRequestArgs {Pr = args.Pr, UserIds = args.Data.ReadData<UserPresenceRequest>().UserIds }
                    );
                    break;

                #endregion

                #region Channels

                case PacketId.ClientChannelJoin:
                    await _evmgr.RunEvent(
                        EventType.BanchoChannelJoin,
                        new BanchoChannelJoinArgs
                        {
                            Pr = args.Pr, ChannelName = args.Data.ReadData<ChannelJoin>().ChannelName
                        }
                    );
                    break;
                case PacketId.ClientChannelLeave:
                    await _evmgr.RunEvent(
                        EventType.BanchoChannelLeave,
                        new BanchoChannelLeaveArgs
                        {
                            Pr = args.Pr, ChannelName = args.Data.ReadData<ChannelLeave>().ChannelName
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
                        new BanchoSendIrcMessageArgs {Pr = args.Pr, Message = msg.Msg}
                    );

                    break;

                #endregion

                #region Friends

                case PacketId.ClientFriendAdd:
                    await _evmgr.RunEvent(
                        EventType.BanchoFriendAdd,
                        new BanchoFriendAddArgs {Pr = args.Pr, FriendId = args.Data.ReadData<FriendAdd>().FriendId}
                    );
                    break;
                case PacketId.ClientFriendRemove:
                    await _evmgr.RunEvent(
                        EventType.BanchoFriendRemove,
                        new BanchoFriendRemoveArgs
                        {
                            Pr = args.Pr, FriendId = args.Data.ReadData<FriendRemove>().FriendId
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
                            Pr = args.Pr, SpectatorHostId = args.Data.ReadData<StartSpectating>().ToSpectateId
                        }
                    );
                    break;
                case PacketId.ClientStopSpectating:
                    await _evmgr.RunEvent(
                        EventType.BanchoStopSpectating,
                        new BanchoStopSpectatingArgs {Pr = args.Pr}
                    );
                    break;
                case PacketId.ClientCantSpectate:
                    await _evmgr.RunEvent(
                        EventType.BanchoCantSpectate,
                        new BanchoCantSpectateArgs {Pr = args.Pr}
                    );
                    break;
                case PacketId.ClientSpectateFrames:
                    await _evmgr.RunEvent(
                        EventType.BanchoBroadcastFrames,
                        new BanchoBroadcastFramesArgs
                        {
                            Pr = args.Pr, Frames = args.Data.ReadData<SpectatorFrames>().Frames
                        }
                    );
                    break;

                #endregion

                #region Multi

                case PacketId.ClientLobbyJoin:
                    await _evmgr.RunEvent(
                        EventType.BanchoLobbyJoin,
                        new BanchoLobbyJoinArgs {Pr = args.Pr}
                    );
                    break;
                case PacketId.ClientLobbyPart:
                    await _evmgr.RunEvent(
                        EventType.BanchoLobbyPart,
                        new BanchoLobbyPartArgs {Pr = args.Pr}
                    );
                    break;
                case PacketId.ClientMatchCreate:
                    var x = args.Data.ReadData<MatchCreate>().Match;

                    await _evmgr.RunEvent(
                        EventType.BanchoMatchCreate,
                        new BanchoMatchCreateArgs {Pr = args.Pr, Room = x}
                    );
                    break;
                case PacketId.ClientMatchJoin:
                    var mJoin = args.Data.ReadData<MatchJoin>();

                    await _evmgr.RunEvent(
                        EventType.BanchoMatchJoin,
                        new BanchoMatchJoinArgs {Pr = args.Pr, MatchId = mJoin.MatchId, Password = mJoin.Password}
                    );
                    break;
                case PacketId.ClientMatchPart:
                    await _evmgr.RunEvent(
                        EventType.BanchoMatchPart,
                        new BanchoMatchPartArgs {Pr = args.Pr}
                    );
                    break;
                case PacketId.ClientMatchChangeSlot:
                    await _evmgr.RunEvent(
                        EventType.BanchoMatchChangeSlot,
                        new BanchoMatchChangeSlotArgs
                        {
                            Pr = args.Pr, SlotId = args.Data.ReadData<MatchChangeSlot>().SlotId
                        }
                    );
                    break;
                case PacketId.ClientMatchChangeMods:
                    await _evmgr.RunEvent(
                        EventType.BanchoMatchChangeMods,
                        new BanchoMatchChangeModsArgs {Pr = args.Pr, Mods = args.Data.ReadData<MatchChangeMods>().Mods}
                    );
                    break;
                case PacketId.ClientMatchLock:
                    await _evmgr.RunEvent(
                        EventType.BanchoMatchLock,
                        new BanchoMatchLockArgs {Pr = args.Pr, SlotId = args.Data.ReadData<MatchLock>().SlotId}
                    );
                    break;
                case PacketId.ClientMatchChangeSettings:
                    var y = args.Data.ReadData<MatchChangeSettings>().Match;
                    
                    await _evmgr.RunEvent(
                        EventType.BanchoMatchChangeSettings,
                        new BanchoMatchChangeSettingsArgs {Pr = args.Pr, Room = y}
                    );
                    break;
                case PacketId.ClientMatchChangePassword:
                    var xy = args.Data.ReadData<MatchChangePassword>().Match;

                    await _evmgr.RunEvent(
                        EventType.BanchoMatchChangePassword,
                        new BanchoMatchChangePasswordArgs {Pr = args.Pr, Room = xy}
                    );
                    break;
                case PacketId.ClientMatchChangeTeam:
                    await _evmgr.RunEvent(
                        EventType.BanchoMatchChangeTeam,
                        new BanchoMatchChangeTeamArgs {Pr = args.Pr}
                    );
                    break;
                case PacketId.ClientMatchReady:
                    await _evmgr.RunEvent(
                        EventType.BanchoMatchReady,
                        new BanchoMatchReadyArgs {Pr = args.Pr}
                    );
                    break;
                case PacketId.ClientMatchNotReady:
                    await _evmgr.RunEvent(
                        EventType.BanchoMatchNotReady,
                        new BanchoMatchNotReadyArgs {Pr = args.Pr}
                    );
                    break;
                case PacketId.ClientMatchTransferHost:
                    await _evmgr.RunEvent(
                        EventType.BanchoMatchTransferHost,
                        new BanchoMatchTransferHostArgs
                        {
                            Pr = args.Pr, SlotId = args.Data.ReadData<MatchTransferHost>().SlotId
                        }
                    );
                    break;
                case PacketId.ClientMatchNoBeatmap:
                    await _evmgr.RunEvent(
                        EventType.BanchoMatchNoBeatmap,
                        new BanchoMatchNoBeatmapArgs {Pr = args.Pr}
                    );
                    break;
                case PacketId.ClientMatchHasBeatmap:
                    await _evmgr.RunEvent(
                        EventType.BanchoMatchHasBeatmap,
                        new BanchoMatchHasBeatmapArgs {Pr = args.Pr}
                    );
                    break;
                case PacketId.ClientInvite:
                    await _evmgr.RunEvent(
                        EventType.BanchoInvite,
                        new BanchoInviteArgs {Pr = args.Pr, UserId = args.Data.ReadData<Invite>().UserId}
                    );
                    break;
                case PacketId.ClientMatchStart:
                    await _evmgr.RunEvent(
                        EventType.BanchoMatchStart,
                        new BanchoMatchStartArgs {Pr = args.Pr}
                    );
                    break;
                case PacketId.ClientMatchLoadComplete:
                    await _evmgr.RunEvent(
                        EventType.BanchoMatchLoadComplete,
                        new BanchoMatchLoadCompleteArgs {Pr = args.Pr}
                    );
                    break;
                case PacketId.ClientMatchScoreUpdate:
                    await _evmgr.RunEvent(
                        EventType.BanchoMatchScoreUpdate,
                        new BanchoMatchScoreUpdateArgs
                        {
                            Pr = args.Pr, Frame = args.Data.ReadData<MatchScoreUpdate>().Frame
                        }
                    );
                    break;
                case PacketId.ClientMatchFailed:
                    await _evmgr.RunEvent(
                        EventType.BanchoMatchFailed,
                        new BanchoMatchFailedArgs {Pr = args.Pr}
                    );
                    break;
                case PacketId.ClientMatchSkipRequest:
                    await _evmgr.RunEvent(
                        EventType.BanchoMatchSkipRequest,
                        new BanchoMatchSkipRequestArgs {Pr = args.Pr}
                    );
                    break;
                case PacketId.ClientMatchComplete:
                    await _evmgr.RunEvent(
                        EventType.BanchoMatchComplete,
                        new BanchoMatchCompleteArgs {Pr = args.Pr}
                    );
                    break;

                #endregion

                #region Other

                case PacketId.ClientExit:
                    await _evmgr.RunEvent(
                        EventType.BanchoExit,
                        new BanchoExitArgs {Pr = args.Pr, Err = args.Data.ReadData<Exit>().ErrorState}
                    );
                    break;
                case PacketId.ClientBeatmapInfoRequest:
                    var data = args.Data.ReadData<BeatmapInfoRequest>();
                    await _evmgr.RunEvent(
                        EventType.BanchoBeatmapInfoRequest,
                        new BanchoBeatmapInfoRequestArgs{Pr = args.Pr, FileNames = data.FileNames});
                    break;
                default:
                    Logger.Debug(
                        $"PacketId: {args.PacketId} Length: {args.Data.BaseStream.Length} Data: {Hex.ToHex(args.Data.ReadToEnd())}"
                    );
                    break;

                #endregion
            }
        }
    }
}
