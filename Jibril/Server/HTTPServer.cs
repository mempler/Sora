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
using System.Threading;
using System.Threading.Tasks;
using EventManager.Enums;
using EventManager.Interfaces;
using Jibril.EventArgs;
using Jibril.Helpers;
using Shared.Helpers;
using SimpleHttp;

namespace Jibril.Server
{
    #region Server

    public class HttpServer
    {
        private readonly EventManager.EventManager _evmng;
        private readonly short _port;
        private bool _running;
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        public HttpServer(Config cfg, EventManager.EventManager evmng)
        {
            _evmng = evmng;
            _port    = cfg.Server.Port == 0 ? (short) 13415 : cfg.Server.Port;
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
            Route.Add("/web/osu-osz2-getscores.php", (req, res, args) =>
            {
                IEventArgs arg = new SharedEventArgs {req = req, res = res, args = args};
                _evmng.RunEvent(EventType.SharedScoreboardRequest, arg);
                res.Close();
            },"GET");

            Route.Add("/web/osu-submit-modular.php", (req, res, args) =>
            {
                IEventArgs arg = new SharedEventArgs {req = req, res = res, args = args};
                _evmng.RunEvent(EventType.SharedScoreSubmittion, arg);
                res.Close();
            }, "GET");

            Route.Add("/web/osu-submit-modular.php", (req, res, args) =>
            {
                IEventArgs arg = new SharedEventArgs {req = req, res = res, args = args};
                _evmng.RunEvent(EventType.SharedScoreSubmittion, arg);
                res.Close();
            }, "POST");

            Route.Add("/web/osu-getreplay.php", (req, res, args) =>
            {
                IEventArgs arg = new SharedEventArgs {req = req, res = res, args = args};
                _evmng.RunEvent(EventType.SharedGetReplay, arg);
                res.Close();
            }, "GET");

            Route.Add("/{avatar}", (request, response, args) =>
            {
                IEventArgs arg = new SharedEventArgs {req = request, res = response, args = args};
                
                if (request.Url.Host.StartsWith("a."))
                    _evmng.RunEvent(EventType.SharedAvatars, arg);
                else
                    response.StatusCode = 404;

                response.Close();
            }, "GET");

            Route.Error = (req, res, ex) =>
            {
                Logger.Info($"Unknown Path {req.Url.AbsolutePath} Method is {req.HttpMethod}");
                res.Close();
            };

            await SimpleHttp.HttpServer.ListenAsync(_port, _cts.Token, Route.OnHttpRequestAsync);
        }
    }

    #endregion
}