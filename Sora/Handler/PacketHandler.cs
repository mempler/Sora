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
                        default:
                            Logger.L.Debug("PacketId: " + packetId);
                            Logger.L.Debug("PacketLength: " + packetData.Length);
                            break;
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