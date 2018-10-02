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
using System.IO;
using Shared.Enums;
using Shared.Handlers;
using Shared.Helpers;
using Sora.Enums;
using Sora.Objects;
using Sora.Packets.Client;
using Sora.Packets.Server;
using Sora.Server;

namespace Sora.Handler
{
    internal class PacketHandler
    {
        [Handler(HandlerTypes.PacketHandler)]
        public void HandlePackets(Req req, Res res)
        {
            try
            {
                if (!req.Headers.ContainsKey("osu-token"))
                {
                    Handlers.ExecuteHandler(HandlerTypes.LoginHandler, req, res);
                    return;
                }
            }
            catch (Exception ex)
            {
                Logger.L.Error(ex);
                res.Writer.Write(new LoginResponse(LoginResponses.Exception));
                return;
            }

            var pr = Presences.GetPresence(req.Headers["osu-token"]);
            if (pr == null)
            {
                res.StatusCode = 403;
                return;
            }

            while (true)
            {
                try
                {
                    if (req.Reader.BaseStream.Position - req.Reader.BaseStream.Length < 0x0b) break; // Dont handle any invalid packets! (less then bytelength of 0x0b)
                    var packetId = (PacketId) req.Reader.ReadInt16();
                    req.Reader.ReadBoolean();
                    var packetData = req.Reader.ReadBytes();
                    var packetDataReader = new MStreamReader(new MemoryStream(packetData));

                    switch (packetId)
                    {
                        case PacketId.ClientSendUserStatus:
                            var sendUserStatus = new SendUserStatus();
                            sendUserStatus.ReadFromStream(packetDataReader);
                            Handlers.ExecuteHandler(HandlerTypes.ClientSendUserStatus, pr, sendUserStatus.Status);
                            break;
                        case PacketId.ClientPong:
                            Handlers.ExecuteHandler(HandlerTypes.ClientPong, pr);
                            break;
                        case PacketId.ClientRequestStatusUpdate:
                            Handlers.ExecuteHandler(HandlerTypes.ClientRequestStatusUpdate, pr);
                            break;
                        case PacketId.ClientUserStatsRequest:
                            var x = new UserStatsRequest();
                            x.ReadFromStream(packetDataReader);
                            Handlers.ExecuteHandler(HandlerTypes.ClientSendUserStatus, pr, x.Userids);
                            break;
                        case PacketId.ClientSendIrcMessage:
                            break;
                        case PacketId.ClientExit:
                            break;
                        case PacketId.ClientStartSpectating:
                            break;
                        case PacketId.ClientStopSpectating:
                            break;
                        case PacketId.ClientSpectateFrames:
                            break;
                        case PacketId.ClientErrorReport:
                            break;
                        case PacketId.ClientCantSpectate:
                            break;
                        case PacketId.ClientSendIrcMessagePrivate:
                            break;

                        /* Multi */
                        case PacketId.ClientLobbyPart:
                            break;
                        case PacketId.ClientLobbyJoin:
                            break;
                        case PacketId.ClientMatchCreate:
                            break;
                        case PacketId.ClientMatchJoin:
                            break;
                        case PacketId.ClientMatchPart:
                            break;
                        case PacketId.ClientMatchChangeSlot:
                            break;
                        case PacketId.ClientMatchReady:
                            break;
                        case PacketId.ClientMatchLock:
                            break;
                        case PacketId.ClientMatchChangeSettings:
                            break;
                        case PacketId.ClientMatchStart:
                            break;
                        case PacketId.ClientMatchScoreUpdate:
                            break;
                        case PacketId.ClientMatchComplete:
                            break;
                        case PacketId.ClientMatchChangeMods:
                            break;
                        case PacketId.ClientMatchLoadComplete:
                            break;
                        case PacketId.ClientMatchNoBeatmap:
                            break;
                        case PacketId.ClientMatchNotReady:
                            break;
                        case PacketId.ClientMatchFailed:
                            break;
                        case PacketId.ClientMatchHasBeatmap:
                            break;
                        case PacketId.ClientMatchSkipRequest:
                            break;

                        /* Multi End */
                        case PacketId.ClientChannelJoin:
                            Handlers.ExecuteHandler(HandlerTypes.ClientChannelJoin, packetDataReader.ReadString());
                            break;
                        case PacketId.ClientBeatmapInfoRequest:
                            break;
                        case PacketId.ClientMatchTransferHost:
                            break;
                        case PacketId.ClientFriendAdd:
                            break;
                        case PacketId.ClientFriendRemove:
                            break;
                        case PacketId.ClientMatchChangeTeam:
                            break;
                        case PacketId.ClientChannelLeave:
                            break;
                        case PacketId.ClientReceiveUpdates:
                            break;
                        case PacketId.ClientSetIrcAwayMessage:
                            break;
                        case PacketId.ClientInvite:
                            break;
                        case PacketId.ClientMatchChangePassword:
                            break;
                        case PacketId.ClientSpecialMatchInfoRequest:
                            break;
                        case PacketId.ClientUserPresenceRequest:
                            break;
                        case PacketId.ClientUserPresenceRequestAll:
                            break;
                        case PacketId.ClientUserToggleBlockNonFriendPm:
                            break;
                        case PacketId.ClientMatchAbort:
                            break;
                        case PacketId.ClientSpecialJoinMatchChannel:
                            break;
                        case PacketId.ClientSpecialLeaveMatchChannel:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                catch
                {
                    break;
                }
            }
            if (pr.LastRequest)
                Presences.EndPresence(pr, true);
        }
    }
}