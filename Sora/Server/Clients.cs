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
using System.IO;
using System.Net;
using EventManager.Enums;
using Shared.Enums;
using Shared.Helpers;
using Sora.Enums;
using Sora.EventArgs;
using Sora.Objects;
using Sora.Packets.Server;
using Sora.Services;

namespace Sora.Server
{
    public abstract class Client
    {        
        protected Client(HttpListenerRequest request, HttpListenerResponse response)
        {
            Request  = request;
            Response = response;
        }

        public HttpListenerRequest Request { get; }
        public HttpListenerResponse Response { get; }


        public abstract void DoWork();
    }

    public class BrowserClient : Client
    {
        public BrowserClient(HttpListenerRequest request, HttpListenerResponse response)
            : base(request, response)
        {
        }

        public override void DoWork()
        {
            Console.WriteLine("Browser connection");
            Console.WriteLine(Request.Url);
            Response.Close();
        }
    }

    public class OsuClient : Client
    {
        private readonly ChannelService _cs;
        private readonly EventManager.EventManager _evmng;
        private readonly PresenceService _ps;

        public OsuClient(HttpListenerRequest request, HttpListenerResponse response,
                         ChannelService cs,
                         EventManager.EventManager evmng,
                         PresenceService ps)
            : base(request, response)
        {
            _cs = cs;
            _evmng = evmng;
            _ps = ps;
        }

        public override void DoWork()
        {
            try
            {
                using (MemoryStream rawReadstream = new MemoryStream())
                using (MStreamWriter mw = new MStreamWriter(Response.OutputStream))
                using (MStreamReader mr = new MStreamReader(rawReadstream))
                {
                    Request.InputStream.CopyTo(rawReadstream);
                    rawReadstream.Position = 0;

                    Presence pr;
                    try
                    {
                        if (Request.Headers["osu-token"] == null || Request.Headers["osu-token"] == string.Empty)
                        {
                            pr                            = new Presence(_cs);
                            Response.Headers["cho-token"] = pr.Token;
                            string ip                        = Response.Headers["X-Forwarded-For"];
                            if (string.IsNullOrEmpty(ip)) ip = "127.0.0.1";
                            _evmng.RunEvent(EventType.BanchoLoginRequest, new BanchoLoginRequestArgs()
                            {
                                pr = pr, Reader = mr, Writer = mw, IPAddress = ip
                            });
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Err(ex);
                        mw.Write(new LoginResponse(LoginResponses.Exception));
                        return;
                    }

                    pr = _ps.GetPresence(Request.Headers["osu-token"]);
                    if (pr == null)
                    {
                        Response.StatusCode = 403; // Presence is not known, force the client to send login info.
                        return;
                    }

                    while (true)
                        try
                        {
                            if (Request.ContentLength64 - rawReadstream.Position < 7)
                                break; // Dont handle any invalid packets! (less then bytelength of 7)

                            PacketId packetId = (PacketId) mr.ReadInt16();
                            mr.ReadBoolean();
                            byte[] packetData = mr.ReadBytes();

                            using (MemoryStream packetDataStream = new MemoryStream(packetData))
                            using (MStreamReader packetDataReader = new MStreamReader(packetDataStream))
                            {
                                _evmng.RunEvent(EventType.BanchoPacket, new BanchoPacketArgs()
                                {
                                    pr = pr, PacketId = packetId, Data = packetDataReader
                                });
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Err(ex);
                            break;
                        }

                    try
                    {
                        if (Response.OutputStream.CanWrite)
                            pr.GetOutput()
                              .WriteTo(Response.OutputStream);
                        Response.Close();
                    }
                    catch
                    {
                        // Ignored because it may throw an exception.
                    }

                    if (pr.IsLastRequest)
                        _ps.EndPresence(pr, true);
                }
            }
            catch (Exception ex)
            {
                Logger.Err(ex);
            }
        }
    }
}