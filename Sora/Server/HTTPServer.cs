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
using System.Threading;
using System.Threading.Tasks;
using SimpleHttp;

namespace Sora.Server
{
    #region Server

    public class HttpServer
    {
        private readonly short _port;
        private bool _running;
        private readonly CancellationTokenSource cts = new CancellationTokenSource();

        public HttpServer(short port = 5001)
        {
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
            cts.Cancel();
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
                    client = new OsuClient(req, res);
                else
                    client = new BrowserClient(req, res);

                client.DoWork();
            }, "POST");

            Route.Add("/", (req, res, args) =>
            {
                Client client;
                if (req.UserAgent == "osu!")
                    client = new OsuClient(req, res);
                else
                    client = new BrowserClient(req, res);

                client.DoWork();
            });

            await SimpleHttp.HttpServer.ListenAsync(_port, cts.Token, Route.OnHttpRequestAsync);
        }
    }

    #endregion
}