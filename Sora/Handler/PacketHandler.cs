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

using Shared.Enums;
using Shared.Handlers;
using Shared.Helpers;
using Sora.Objects;
using Sora.Packets.Client;

namespace Sora.Handler
{
    internal class PacketHandler
    {
        [Handler(HandlerTypes.BanchoPacketHandler)]
        public void HandlePackets(Presence pr, PacketId packetId, MStreamReader data)
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (packetId)
            {
                #region Stats

                case PacketId.ClientSendUserStatus:
                    Handlers.ExecuteHandler(HandlerTypes.BanchoSendUserStatus, pr,
                                            data.ReadData<SendUserStatus>().Status);
                    break;
                case PacketId.ClientPong:
                    Handlers.ExecuteHandler(HandlerTypes.BanchoPong, pr);
                    break;
                case PacketId.ClientRequestStatusUpdate:
                    Handlers.ExecuteHandler(HandlerTypes.BanchoRequestStatusUpdate, pr);
                    break;
                case PacketId.ClientUserStatsRequest:
                    Handlers.ExecuteHandler(HandlerTypes.BanchoUserStatsRequest, pr,
                                            data.ReadData<UserStatsRequest>().Userids);
                    break;
                case PacketId.ClientReceiveUpdates:
                    Handlers.ExecuteHandler(HandlerTypes.BanchoReceiveUpdates, pr,
                                            data.ReadData<Packets.Client.ReceiveUpdates>().UserId);
                    break;

                #endregion

                #region Channels

                case PacketId.ClientChannelJoin:
                    Handlers.ExecuteHandler(HandlerTypes.BanchoChannelJoin, pr,
                                            data.ReadData<ChannelJoin>().ChannelName);
                    break;
                case PacketId.ClientChannelLeave:
                    Handlers.ExecuteHandler(HandlerTypes.BanchoChannelLeave, pr,
                                            data.ReadData<ChannelLeave>().ChannelName);
                    break;

                #endregion

                #region Chat

                case PacketId.ClientSendIrcMessagePrivate:
                case PacketId.ClientSendIrcMessage:
                    SendIrcMessage msg = data.ReadData<SendIrcMessage>();
                    if (msg.Msg.ChannelTarget.StartsWith("#"))
                        Handlers.ExecuteHandler(HandlerTypes.BanchoSendIrcMessage, pr, msg.Msg);
                    else
                        Handlers.ExecuteHandler(HandlerTypes.BanchoSendIrcMessagePrivate, pr, msg.Msg);
                    break;

                #endregion

                #region Friends

                case PacketId.ClientFriendAdd:
                    Handlers.ExecuteHandler(HandlerTypes.BanchoFriendAdd, pr,
                                            data.ReadData<FriendAdd>().FriendId);
                    break;
                case PacketId.ClientFriendRemove:
                    Handlers.ExecuteHandler(HandlerTypes.BanchoFriendRemove, pr,
                                            data.ReadData<FriendRemove>().FriendId);
                    break;

                #endregion

                #region Spectator

                case PacketId.ClientStartSpectating:
                    Handlers.ExecuteHandler(HandlerTypes.BanchoStartSpectating, pr,
                                            data.ReadData<StartSpectating>().ToSpectateId);
                    break;
                case PacketId.ClientStopSpectating:
                    Handlers.ExecuteHandler(HandlerTypes.BanchoStopSpectating, pr);
                    break;
                case PacketId.ClientCantSpectate:
                    Handlers.ExecuteHandler(HandlerTypes.BanchoCantSpectate, pr);
                    break;
                case PacketId.ClientSpectateFrames:
                    Handlers.ExecuteHandler(HandlerTypes.BanchoSpectateFrames, pr,
                                            data.ReadData<SpectatorFrames>().Frames);
                    break;

                #endregion

                #region Multi

                case PacketId.ClientLobbyJoin:
                    Handlers.ExecuteHandler(HandlerTypes.BanchoLobbyJoin, pr);
                    break;
                case PacketId.ClientLobbyPart:
                    Handlers.ExecuteHandler(HandlerTypes.BanchoLobbyPart, pr);
                    break;
                case PacketId.ClientMatchCreate:
                    Handlers.ExecuteHandler(HandlerTypes.BanchoMatchCreate, pr,
                                            data.ReadData<MatchCreate>().Room);
                    break;
                case PacketId.ClientMatchJoin:
                    MatchJoin mJoin = data.ReadData<MatchJoin>();
                    Handlers.ExecuteHandler(HandlerTypes.BanchoMatchJoin, pr, mJoin.MatchId, mJoin.Password);
                    break;
                case PacketId.ClientMatchPart:
                    Handlers.ExecuteHandler(HandlerTypes.BanchoMatchPart, pr);
                    break;
                case PacketId.ClientMatchChangeSlot:
                    Handlers.ExecuteHandler(HandlerTypes.BanchoMatchChangeSlot, pr,
                                            data.ReadData<MatchChangeSlot>().SlotId);
                    break;
                case PacketId.ClientMatchChangeMods:
                    Handlers.ExecuteHandler(HandlerTypes.BanchoMatchChangeMods, pr,
                                            data.ReadData<MatchChangeMods>().Mods);
                    break;
                case PacketId.ClientMatchLock:
                    Handlers.ExecuteHandler(HandlerTypes.BanchoMatchLock, pr,
                                            data.ReadData<MatchLock>().SlotId);
                    break;
                case PacketId.ClientMatchChangeSettings:
                    Handlers.ExecuteHandler(HandlerTypes.BanchoMatchChangeSettings, pr,
                                            data.ReadData<MatchChangeSettings>().Room);
                    break;
                case PacketId.ClientMatchChangePassword:
                    Handlers.ExecuteHandler(HandlerTypes.BanchoMatchChangePassword, pr,
                                            data.ReadData<MatchChangePassword>().Room);
                    break;
                case PacketId.ClientMatchChangeTeam:
                    Handlers.ExecuteHandler(HandlerTypes.BanchoMatchChangeTeam, pr);
                    break;
                case PacketId.ClientMatchReady:
                    Handlers.ExecuteHandler(HandlerTypes.BanchoMatchReady, pr);
                    break;
                case PacketId.ClientMatchNotReady:
                    Handlers.ExecuteHandler(HandlerTypes.BanchoMatchNotReady, pr);
                    break;
                case PacketId.ClientMatchTransferHost:
                    Handlers.ExecuteHandler(HandlerTypes.BanchoMatchTransferHost, pr,
                                            data.ReadData<MatchTransferHost>().SlotId);
                    break;
                case PacketId.ClientMatchNoBeatmap:
                    Handlers.ExecuteHandler(HandlerTypes.BanchoMatchNoBeatmap, pr);
                    break;
                case PacketId.ClientMatchHasBeatmap:
                    Handlers.ExecuteHandler(HandlerTypes.BanchoMatchHasBeatmap, pr);
                    break;
                case PacketId.ClientInvite:
                    Handlers.ExecuteHandler(HandlerTypes.BanchoInvite, pr, data.ReadData<Invite>().UserId);
                    break;
                case PacketId.ClientMatchStart:
                    Handlers.ExecuteHandler(HandlerTypes.BanchoMatchStart, pr);
                    break;
                case PacketId.ClientMatchLoadComplete:
                    Handlers.ExecuteHandler(HandlerTypes.BanchoMatchLoadComplete, pr);
                    break;
                case PacketId.ClientMatchScoreUpdate:
                    Handlers.ExecuteHandler(HandlerTypes.BanchoMatchScoreUpdate, pr,
                                            data.ReadData<MatchScoreUpdate>().Frame);
                    break;
                case PacketId.ClientMatchFailed:
                    Handlers.ExecuteHandler(HandlerTypes.BanchoMatchFailed, pr);
                    break;
                case PacketId.ClientMatchSkipRequest:
                    Handlers.ExecuteHandler(HandlerTypes.BanchoMatchSkipRequest, pr);
                    break;
                case PacketId.ClientMatchComplete:
                    Handlers.ExecuteHandler(HandlerTypes.BanchoMatchComplete, pr);
                    break;

                #endregion

                #region Other

                case PacketId.ClientExit:
                    Handlers.ExecuteHandler(HandlerTypes.BanchoExit, pr, data.ReadData<Exit>().ErrorState);
                    break;
                default:
                    Logger.L.Debug(
                        $"PacketId: {packetId} Length: {data.BaseStream.Length} Data: {Hex.ToHex(data.ReadToEnd())}");
                    break;

                #endregion
            }
        }
    }
}