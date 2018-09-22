using System;
using Sora.Enums;
using Sora.Objects;
using Sora.Packets.Server;
using Sora.Server;

namespace Sora.Handler
{
    internal class LoginHandler
    {
        [Handler(HandlerTypes.LoginHandler)]
        public void OnLogin(Req req, Res res)
        {
            Console.WriteLine("Done");
            var pr = new Presence();
            res.Headers["cho-token"] = pr.Token;
           
            res.Writer.Write(new LoginResponse(LoginResponses.Failed));
            res.Writer.Write(new Announce("Hello c# 2.0"));
        }
    }
}
