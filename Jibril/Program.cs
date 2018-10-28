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

namespace Jibril
{
    using System;
    using System.Diagnostics;
    using System.Reflection;
    using Server;
    using Shared.Database;
    using Shared.Enums;
    using Shared.Handlers;
    using Shared.Helpers;
    using Shared.Plugins;

    internal static class Program
    {
        private static HttpServer _server;
        
        private static void Initialize()
        {
            try
            {
                Logger.L.Info("Start Initalization");
                Stopwatch watch = Stopwatch.StartNew();
                Config conf = Config.ReadConfig(5002);
                
                _server = new HttpServer(conf.Server.Port);
                using (new SoraContext()) { } // Initialize Database. (Migrate database)

                Loader.LoadPlugins();
                Handlers.InitHandlers(Assembly.GetEntryAssembly(), false);
                Handlers.ExecuteHandler(HandlerTypes.Initializer);

                watch.Stop();
                Logger.L.Info($"Initalization Success. it took {watch.Elapsed.Seconds} second(s)");
            } catch (Exception ex)
            {
                Logger.L.Error(ex);
            }
        }

        [STAThread]
        private static void Main()
        {
            Initialize();
            _server.RunAsync().Wait();
        }
    }
}
