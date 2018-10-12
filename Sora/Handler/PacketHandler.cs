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
using Shared.Server;
using Sora.Enums;
using Sora.Objects;
using Sora.Packets.Client;
using Sora.Packets.Server;
using SendIrcMessage = Sora.Packets.Client.SendIrcMessage;

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

            Presence pr = LPresences.GetPresence(req.Headers["osu-token"]);
            if (pr == null)
            {
                res.StatusCode = 403; // Presence is not known, force the client to send login info.
                return;
            }

            while (true)
                try
                {
                    if (req.Reader.BaseStream.Length - req.Reader.BaseStream.Position < 7)
                        break; // Dont handle any invalid packets! (less then bytelength of 7)
                    PacketId packetId = (PacketId) req.Reader.ReadInt16();
                    req.Reader.ReadBoolean();
                    byte[]        packetData       = req.Reader.ReadBytes();
                    MStreamReader packetDataReader = new MStreamReader(new MemoryStream(packetData));

                    if (packetId != PacketId.ClientPong && packetId != PacketId.ClientUserStatsRequest)
                        Logger.L.Debug(
                            $"Packet: {packetId} Length: {packetData.Length} Data: {BitConverter.ToString(packetData).Replace("-", "")}");

                    // ReSharper disable once SwitchStatementMissingSomeCases
                    switch (packetId)
                    {
                        case PacketId.ClientSendUserStatus:
                            SendUserStatus sendUserStatus = new SendUserStatus();
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
                            UserStatsRequest userStatsRequest = new UserStatsRequest();
                            userStatsRequest.ReadFromStream(packetDataReader);
                            Handlers.ExecuteHandler(HandlerTypes.ClientUserStatsRequest, pr, userStatsRequest.Userids);
                            break;
                        case PacketId.ClientChannelJoin:
                            ChannelJoin channelJoin = new ChannelJoin();
                            channelJoin.ReadFromStream(packetDataReader);
                            Handlers.ExecuteHandler(HandlerTypes.ClientChannelJoin, pr, channelJoin.ChannelName);
                            break;
                        case PacketId.ClientChannelLeave:
                            ChannelLeave channelLeave = new ChannelLeave();
                            channelLeave.ReadFromStream(packetDataReader);
                            Handlers.ExecuteHandler(HandlerTypes.ClientChannelLeave, pr, channelLeave.ChannelName);
                            break;
                        case PacketId.ClientSendIrcMessage:
                            SendIrcMessage msg = new SendIrcMessage();
                            msg.ReadFromStream(packetDataReader);
                            Handlers.ExecuteHandler(HandlerTypes.ClientSendIrcMessage, pr, msg.Msg);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Logger.L.Error(ex);
                    break;
                }

            if (res.Writer.BaseStream.CanWrite)
                pr.GetOutput()
                  .WriteTo(res.Writer.BaseStream);

            if (pr.IsLastRequest)
                LPresences.EndPresence(pr, true);
        }
    }
}