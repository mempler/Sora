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
using System.Net;
using Shared.Enums;
using Shared.Helpers;
using Sora.Enums;
using Sora.Objects;
using Sora.Packets.Server;

namespace Sora.Server
{
public abstract class Client
    {
        protected Client(HttpListenerRequest request, HttpListenerResponse response)
        {
            Request = request;
            Response = response;
        }

        public HttpListenerRequest Request { get; private set; }
        public HttpListenerResponse Response { get; private set; }


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
        public OsuClient(HttpListenerRequest request, HttpListenerResponse response)
            : base(request, response)
        {
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
                            pr                               = new Presence();
                            Response.Headers["cho-token"]    = pr.Token;
                            string ip                        = Response.Headers["X-Forwarded-For"];
                            if (string.IsNullOrEmpty(ip)) ip = "127.0.0.1";

                            Shared.Handlers.Handlers.ExecuteHandler(HandlerTypes.LoginHandler, pr, mw, mr, ip);
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.L.Error(ex);
                        mw.Write(new LoginResponse(LoginResponses.Exception));
                        return;
                    }

                    pr = LPresences.GetPresence(Request.Headers["osu-token"]);
                    if (pr == null)
                    {
                        Response.StatusCode = 403; // Presence is not known, force the client to send login info.
                        return;
                    }

                    while (true)
                    {
                        try
                        {
                            if (Request.ContentLength64 - rawReadstream.Position < 7)
                                break; // Dont handle any invalid packets! (less then bytelength of 7)

                            PacketId packetId = (PacketId) mr.ReadInt16();
                            mr.BaseStream.Position -= 2;

                            Shared.Handlers.Handlers.ExecuteHandler(HandlerTypes.PacketHandler, pr, packetId, mr);
                        }
                        catch (Exception ex)
                        {
                            Logger.L.Error(ex);
                            break;
                        }
                    }
                    
                    try {
                        if (Response.OutputStream.CanWrite)
                            pr.GetOutput()
                              .WriteTo(Response.OutputStream);
                        Response.Close();
                    } catch {
                        // Ignored because it may throw an exception.
                    }

                    if (pr.IsLastRequest)
                        LPresences.EndPresence(pr, true);
                }
            }
            catch (Exception ex)
            {
                Logger.L.Error(ex);
            }
        }
    }
}