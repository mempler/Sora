using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Amib.Threading;
using Sora.Enums;
using Sora.Handler;
using Sora.Helpers;

namespace Sora.Server
{
    public class HttpServer
    {
        private bool _running;
        private readonly TcpListener _listener;
        private readonly SmartThreadPool _pool;
        private Thread _serverThread;

        public HttpServer(short port)
        {
            _running = false;
            _listener = new TcpListener(IPAddress.Any, port);
            _pool = new SmartThreadPool();
        }

        public Thread Run()
        {
            if (_running)
                throw new Exception("Address already in use!");
            var x = new Thread(_RunServer);
            x.Start();
            _serverThread = x;
            return x;
        }

        public void Stop()
        {
            if (_running)
                throw new Exception("Cannot stop while already stopped!");
            
            Thread.Sleep(100);
            _serverThread.Abort();
        }

        private void _RunServer()
        {
            _listener.Start();
            _running = true;
            while (true)
            {
                if (!_running)
                {
                    _listener.Stop();
                    break;
                } 
                var client = _listener.AcceptTcpClient();
                _pool.QueueWorkItem(_HandleClient, client);
            }
        }

        private static void _HandleClient(TcpClient client)
        {
            try
            {
                Program.Logger.Debug("Incomming TCP Connection!");
                using (client)
                {
                    using (var s = client.GetStream())
                    {
                        var buff = new byte[client.ReceiveBufferSize];
                        s.Read(buff, 0, client.ReceiveBufferSize);
                        var httpString = Encoding.UTF8.GetString(buff);

                        var http = httpString.Split('\n');
                        if (http.Length < 3)
                            return;
                        if (!(http[0].StartsWith("GET") || http[0].StartsWith("POST")))
                        {
                            var o = Encoding.UTF8.GetBytes(
                                "HTTP/1.1 400 Bad Request" +
                                "Host: Kaoiji\r\n" +
                                "\r\n" +
                                "Bad Request!"
                            );
                            s.Write(o, 0, o.Length);
                            return;
                        }

                        if (http[0].StartsWith("GET"))
                        {
                            byte[] o;
                            if (File.Exists("Resource/index.html"))
                            {
                                o = Encoding.UTF8.GetBytes(
                                    "HTTP/1.1 200 OK\r\n" +
                                    "Host: Kaoiji\r\n" +
                                    "\r\n" +
                                    File.ReadAllText("Resource/index.html")
                                );
                            }
                            else
                            {
                                o = Encoding.UTF8.GetBytes(
                                    "HTTP/1.1 200 OK\r\n" +
                                    "Host: Kaoiji\r\n" +
                                    "\r\n" +
                                    "Sora a C# Written Bancho!"
                                );
                            }
                            s.Write(o, 0, o.Length);
                            return;
                        }

                        var readHeaders = true;
                        var r = new Req();
                        foreach (var hs in http)
                        {
                            if (hs.Trim().StartsWith("POST") && hs.Trim().EndsWith("HTTP/1.1")) continue;
                            if (hs.Trim() == "") readHeaders = false;
                            if (!readHeaders) continue;
                            var vx = hs.Split(':');
                            if (vx.Length < 2) continue;
                            r.Headers[vx[0].Trim()] = vx[1].Trim();
                        }

                        var ts = new MemoryStream();
                        var x = new Res
                        {
                            StatusCode = 404, // as default
                            Writer = new MStreamWriter(ts)
                        };

                        Handlers.ExecuteHandler(HandlerTypes.PacketHandler, r, x);

                        using (var outputstream = new MStreamWriter(s))
                        {
                            var oxx = Encoding.UTF8.GetBytes(
                                $"HTTP/1.1 {GetStatus(x)}\r\n" +
                                Header2Str(x)
                            );
                            outputstream.WriteRawBuffer(oxx);
                            x.Writer.BaseStream.Position = 0;
                            x.Writer.BaseStream.CopyTo(outputstream.BaseStream);
                            x.Writer.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Program.Logger.Error(ex);
            }
        }

        private static string Header2Str(Res s)
        {
            var outputStr = string.Empty;
            s.Headers["Host"] = "Bancho";
            foreach (var key in s.Headers.Keys)
                outputStr += $"{key}: ${s.Headers[key]}\r\n";
            outputStr += "\n";
            return outputStr;
        }

        private static string GetStatus(Res s)
        {
            var outputStr = $"{s.StatusCode} Unknown";
            return outputStr;
        }
    }

    public class Req
    {
        public Dictionary<string, string> Headers = new Dictionary<string, string>();
        public MStreamReader Reader;
    }

    public class Res
    {
        public Dictionary<string, string> Headers = new Dictionary<string, string>();
        public int StatusCode { get; set; }
        public MStreamWriter Writer;
    }
}
