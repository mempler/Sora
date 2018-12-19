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
using Shared.Enums;
using Shared.Handlers;
using Shared.Helpers;
using SimpleHttp;

namespace Jibril.Server
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
            Route.Add("/web/{handler}", (request, response, args) =>
            {
                Logger.L.Info(
                    $"Unknown Path {request.Url.AbsolutePath} " +
                    $"Method is {request.HttpMethod} " +
                    $"Query is {request.Url.Query} " +
                    "Args are " + args["handler"]);
                switch (args["handler"].Split("?")[0])
                {
                    case "osu-osz2-getscores.php":
                        Handlers.ExecuteHandler(HandlerTypes.SharedScoreboardRequest, request, response, args);
                        break;
                    case "osu-submit-modular.php":
                        Handlers.ExecuteHandler(HandlerTypes.SharedScoreSubmittion, request, response, args);
                        break;
                    default:
                        Logger.L.Info(
                            $"Unknown Path {request.Url.AbsolutePath} " +
                            $"Method is {request.HttpMethod} " +
                            $"Query is {request.Url.Query} " +
                            "Args are " + args["handler"]);
                        break;
                }

                response.Close();
            });

            Route.Add("/web/{handler}", (request, response, args) =>
            {
                switch (args["handler"].Split("?")[0])
                {
                    case "osu-submit-modular.php":
                        Handlers.ExecuteHandler(HandlerTypes.SharedScoreSubmittion, request, response, args);
                        break;
                    default:
                        Logger.L.Info(
                            $"Unknown Path {request.Url.AbsolutePath} " +
                            $"Method is {request.HttpMethod} " +
                            $"Query is {request.Url.Query} " +
                            "Args are " + args["handler"]);
                        break;
                }

                response.Close();
            }, "POST");

            Route.Add("/{avatar}", (request, response, args) =>
            {
                if (request.Url.Host.StartsWith("a."))
                    Handlers.ExecuteHandler(HandlerTypes.SharedAvatars, request, response, args);
                else
                    response.StatusCode = 404;

                response.Close();
            });

            Route.Error = (request, response, exception) =>
            {
                Logger.L.Info($"Unknown Path {request.Url.AbsolutePath} Method is {request.HttpMethod}");
                response.Close();
            };

            await SimpleHttp.HttpServer.ListenAsync(_port, cts.Token, Route.OnHttpRequestAsync);
        }
    }

    #endregion
}