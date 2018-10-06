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
using JetBrains.Annotations;
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
        [UsedImplicitly]
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
                    if (req.Reader.BaseStream.Length - req.Reader.BaseStream.Position < 7) break; // Dont handle any invalid packets! (less then bytelength of 0x07)
                    var packetId = (PacketId) req.Reader.ReadInt16();
                    req.Reader.ReadBoolean();
                    var packetData = req.Reader.ReadBytes();
                    var packetDataReader = new MStreamReader(new MemoryStream(packetData));

                    if (packetId != PacketId.ClientPong && packetId != PacketId.ClientUserStatsRequest)
                        Logger.L.Debug($"Packet: {packetId} Length: {packetData.Length} Data: {BitConverter.ToString(packetData).Replace("-","")}");
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
                            var userStatsRequest = new UserStatsRequest();
                            userStatsRequest.ReadFromStream(packetDataReader);
                            Handlers.ExecuteHandler(HandlerTypes.ClientUserStatsRequest, pr, userStatsRequest.Userids);
                            break;
                        case PacketId.ClientChannelJoin:
                            var channelJoin = new ChannelJoin();
                            channelJoin.ReadFromStream(packetDataReader);
                            Handlers.ExecuteHandler(HandlerTypes.ClientChannelJoin, pr, channelJoin.ChannelName);
                            break;
                        case PacketId.ClientChannelLeave:
                            var channelLeave = new ChannelLeave();
                            channelLeave.ReadFromStream(packetDataReader);
                            Handlers.ExecuteHandler(HandlerTypes.ClientChannelLeave, pr, channelLeave.ChannelName);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Logger.L.Error(ex);
                    break;
                }
            }
            
            pr.GetOutput()
                .WriteTo(res.Writer.BaseStream);

            if (pr.IsLastRequest)
                Presences.EndPresence(pr, true);
        }
    }
}