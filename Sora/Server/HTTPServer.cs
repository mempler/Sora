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

/*
using System;
using System.Threading;
using System.Threading.Tasks;
using Shared.Helpers;
using Sora.Services;

namespace Sora.Server
{
    #region Server

    public class HttpServer
    {
        private readonly ChannelService _cs;
        private readonly EventManager.EventManager _evmng;
        private readonly PresenceService _ps;
        private readonly short _port;
        private bool _running;
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        public HttpServer(ChannelService cs, EventManager.EventManager evmng, PresenceService ps, short port = 5001)
        {
            _cs = cs;
            _evmng = evmng;
            _ps = ps;
            _port    = port;
            _running = false;
        }

        public async Task RunAsync()
        {
            if (_running)
                throw new Exception("Server is already running!");

            await _RunServer();
        }

        public void Stop()
        {
            if (!_running)
                throw new Exception("Cannot stop if already stopped!");

            _running = false;
            _cts.Cancel();
        }

        private async Task _RunServer()
        {
            Route.Add("/", (req, res, args) =>
            {
                res.Headers["cho-protocol"] = "19";
                res.Headers["Connection"]   = "keep-alive";
                res.Headers["Keep-Alive"]   = "timeout=60, max=100";
                res.Headers["Content-Type"] = "text/html; charset=UTF-8";
                res.Headers["cho-server"]   = "Sora (https://github.com/Mempler/Sora)";

                Client client;
                if (req.UserAgent == "osu!")
                    client = new OsuClient(req, res, _cs, _evmng, _ps);
                else
                    client = new BrowserClient(req, res);

                client.DoWork();
            }, "POST");

            Route.Add("/", (req, res, args) =>
            {
                Client client;
                if (req.UserAgent == "osu!")
                    client = new OsuClient(req, res, _cs, _evmng, _ps);
                else
                    client = new BrowserClient(req, res);

                client.DoWork();
            });

            Logger.Info("Finish Server boot. Server should listen at port%#3cfc59%", _port);
            await SimpleHttp.HttpServer.ListenAsync(_port, _cts.Token, Route.OnHttpRequestAsync);
        }
    }

    #endregion
}

*/