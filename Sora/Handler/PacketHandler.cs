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
using SendIrcMessage = Sora.Packets.Client.SendIrcMessage;

namespace Sora.Handler
{
    internal class PacketHandler
    {
        [Handler(HandlerTypes.PacketHandler)]
        public void HandlePackets(Presence pr, PacketId packetId, MStreamReader data)
        {
            switch (packetId)
            {
                #region Stats
                case PacketId.ClientSendUserStatus:
                    Handlers.ExecuteHandler(HandlerTypes.ClientSendUserStatus, pr,
                                            data.ReadPacketData<SendUserStatus>().Status);
                    break;
                case PacketId.ClientPong:
                    Handlers.ExecuteHandler(HandlerTypes.ClientPong, pr);
                    break;
                case PacketId.ClientRequestStatusUpdate:
                    Handlers.ExecuteHandler(HandlerTypes.ClientRequestStatusUpdate, pr);
                    break;
                case PacketId.ClientUserStatsRequest:
                    Handlers.ExecuteHandler(HandlerTypes.ClientUserStatsRequest, pr,
                                            data.ReadPacketData<UserStatsRequest>().Userids);
                    break;
                case PacketId.ClientReceiveUpdates:
                    Handlers.ExecuteHandler(HandlerTypes.ClientReceiveUpdates, pr,
                                            data.ReadPacketData<Packets.Client.ReceiveUpdates>().UserId);
                    break;

                #endregion
                
                #region Channels
                case PacketId.ClientChannelJoin:
                    Handlers.ExecuteHandler(HandlerTypes.ClientChannelJoin, pr,
                                            data.ReadPacketData<ChannelJoin>().ChannelName);
                    break;
                case PacketId.ClientChannelLeave:
                    Handlers.ExecuteHandler(HandlerTypes.ClientChannelLeave, pr,
                                            data.ReadPacketData<ChannelLeave>().ChannelName);
                    break;
                #endregion
                
                #region Chat
                case PacketId.ClientSendIrcMessagePrivate:
                case PacketId.ClientSendIrcMessage:
                    SendIrcMessage msg = data.ReadPacketData<SendIrcMessage>();
                    if (msg.Msg.ChannelTarget.StartsWith("#"))
                        Handlers.ExecuteHandler(HandlerTypes.ClientSendIrcMessage, pr, msg.Msg);
                    else
                        Handlers.ExecuteHandler(HandlerTypes.ClientSendIrcMessagePrivate, pr, msg.Msg);
                    break;
                #endregion
                
                #region Friends
                case PacketId.ClientFriendAdd:
                    Handlers.ExecuteHandler(HandlerTypes.ClientFriendAdd, pr,
                                            data.ReadPacketData<FriendAdd>().FriendId);
                    break;
                case PacketId.ClientFriendRemove:
                    Handlers.ExecuteHandler(HandlerTypes.ClientFriendRemove, pr,
                                            data.ReadPacketData<FriendRemove>().FriendId);
                    break;
                #endregion
                
                #region Spectator
                case PacketId.ClientStartSpectating:
                    Handlers.ExecuteHandler(HandlerTypes.ClientStartSpectating, pr,
                                            data.ReadPacketData<StartSpectating>().ToSpectateId);
                    break;
                case PacketId.ClientStopSpectating:
                    Handlers.ExecuteHandler(HandlerTypes.ClientStopSpectating, pr);
                    break;
                case PacketId.ClientCantSpectate:
                    Handlers.ExecuteHandler(HandlerTypes.ClientCantSpectate, pr);
                    break;
                case PacketId.ClientSpectateFrames:
                    Handlers.ExecuteHandler(HandlerTypes.ClientSpectateFrames, pr,
                                            data.ReadPacketData<SpectatorFrames>().Frames);
                    break;
                #endregion
                
                #region Multi
                case PacketId.ClientLobbyJoin:
                    Handlers.ExecuteHandler(HandlerTypes.ClientLobbyJoin, pr);
                    break;
                case PacketId.ClientLobbyPart:
                    Handlers.ExecuteHandler(HandlerTypes.ClientLobbyPart, pr);
                    break;
                case PacketId.ClientMatchCreate:
                    Handlers.ExecuteHandler(HandlerTypes.ClientMatchCreate, pr,
                                            data.ReadPacketData<MatchCreate>().Room);
                    break;
                case PacketId.ClientMatchJoin:
                    MatchJoin mJoin = data.ReadPacketData<MatchJoin>();
                    Handlers.ExecuteHandler(HandlerTypes.ClientMatchJoin, pr, mJoin.MatchId, mJoin.Password);
                    break;
                case PacketId.ClientMatchPart:
                    Handlers.ExecuteHandler(HandlerTypes.ClientMatchPart, pr);
                    break;
                case PacketId.ClientMatchChangeSlot:
                    Handlers.ExecuteHandler(HandlerTypes.ClientMatchChangeSlot, pr,
                                            data.ReadPacketData<MatchChangeSlot>().SlotId);
                    break;
                case PacketId.ClientMatchChangeMods:
                    Handlers.ExecuteHandler(HandlerTypes.ClientMatchChangeMods, pr,
                                            data.ReadPacketData<MatchChangeMods>().Mods);
                    break;
                case PacketId.ClientMatchLock:
                    Handlers.ExecuteHandler(HandlerTypes.ClientMatchLock, pr,
                                            data.ReadPacketData<MatchLock>().SlotId);
                    break;
                case PacketId.ClientMatchChangeSettings:
                    Handlers.ExecuteHandler(HandlerTypes.ClientMatchChangeSettings, pr,
                                            data.ReadPacketData<MatchChangeSettings>().Room);
                    break;
                case PacketId.ClientMatchChangePassword:
                    Handlers.ExecuteHandler(HandlerTypes.ClientMatchChangePassword, pr,
                                            data.ReadPacketData<MatchChangePassword>().Room);
                    break;
                case PacketId.ClientMatchChangeTeam:
                    Handlers.ExecuteHandler(HandlerTypes.ClientMatchChangeTeam, pr);
                    break;
                case PacketId.ClientMatchReady:
                    Handlers.ExecuteHandler(HandlerTypes.ClientMatchReady, pr);
                    break;
                case PacketId.ClientMatchNotReady:
                    Handlers.ExecuteHandler(HandlerTypes.ClientMatchNotReady, pr);
                    break;
                case PacketId.ClientMatchTransferHost:
                    Handlers.ExecuteHandler(HandlerTypes.ClientMatchTransferHost, pr,
                                            data.ReadPacketData<MatchTransferHost>().SlotId);
                    break;
                case PacketId.ClientMatchNoBeatmap:
                    Handlers.ExecuteHandler(HandlerTypes.ClientMatchNoBeatmap, pr);
                    break;
                case PacketId.ClientMatchHasBeatmap:
                    Handlers.ExecuteHandler(HandlerTypes.ClientMatchHasBeatmap, pr);
                    break;
                case PacketId.ClientInvite:
                    Handlers.ExecuteHandler(HandlerTypes.ClientInvite, pr,
                                            data.ReadPacketData<Invite>().UserId);
                    break;
                case PacketId.ClientMatchStart:
                    Handlers.ExecuteHandler(HandlerTypes.ClientMatchStart, pr);
                    break;
                case PacketId.ClientMatchLoadComplete:
                    
                #endregion
    
                #region Other

                case PacketId.ClientExit:
                    Handlers.ExecuteHandler(HandlerTypes.ClientExit, pr,
                                            data.ReadPacketData<Exit>().ErrorState);
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