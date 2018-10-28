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

namespace Jibril.Server
{
    using System;
    using System.Net;
    using System.Threading;
    using Amib.Threading;

#region Server

    public class HttpServer
    {
        private readonly HttpListener _listener;
        private readonly SmartThreadPool _pool;
        private bool _running;

        public HttpServer(string hostname = "localhost", short port = 5002)
        {
            _running = false;
            _listener = new HttpListener();
            _listener.Prefixes.Add($"http://{hostname}:{port}/");
            _pool = new SmartThreadPool(
                60,
                Environment.ProcessorCount * 16 +
                1); // 4 * 16 = 64 + 1 for ServerThread (1 core should handle 16 client connections)
        }

        public void Run()
        {
            if (_running)
                throw new Exception("Server is already running!");

            _pool.Start();
            new Thread(_RunServer).Start();
        }

        public void Stop()
        {
            if (!_running)
                throw new Exception("Cannot stop if already stopped!");

            _running = false;
            _pool.Shutdown(true, TimeSpan.FromMinutes(1));
            _listener.Stop();
        }

        private void _RunServer()
        {
            _listener.Start();
            _running = true;

            while (_running)
            {
                if (!_running) break;
                HttpListenerContext context = _listener.GetContext();

                context.Response.Headers["Connection"] = "keep-alive";
                context.Response.Headers["Keep-Alive"] = "timeout=60, max=100";
                context.Response.Headers["Content-Type"] = "text/html; charset=UTF-8";
                context.Response.Headers["cho-server"] = "Jibril (https://github.com/Mempler/Sora)";

                _pool.QueueWorkItem(new BrowserClient(context.Request, context.Response).DoWork);
            }
        }
    }

#endregion
}
