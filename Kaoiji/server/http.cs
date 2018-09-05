using System;
using System.Threading;
using System.Net;
using Amib.Threading;

using Kaoiji.enums;
using Kaoiji.handler;
using System.Collections.Generic;
using System.IO;
using Kaoiji.helpers;
using Kaoiji.consts;
using System.Reflection;
using System.Text;
using Kaoiji.objects;

namespace Kaoiji.server
{
    public class HttpServer
    {
        public static Stream IndexPage { get; set; } =
            Assembly.GetExecutingAssembly()
                    .GetManifestResourceStream("Kaoiji.Resources.index.html");
        private readonly int Port = 5001;
        private bool ShouldStop = false;
        private SmartThreadPool pool;

        public HttpServer(int port) => Port = port;
        public void Stop() => ShouldStop = true;

        /// <summary>
        /// Start the HTTPServer.
        /// </summary>
        public Thread Run()
        {
            pool = new SmartThreadPool();
            Thread t = new Thread(RunServer);
            t.Start();
            return t;
        }

        /// <summary>
        /// Root page handler. "/"
        /// </summary>
        private void Root(HttpListenerRequest r, HttpListenerResponse w)
        {
            w.StatusCode = 200;
            if (r.HttpMethod.ToUpper() != "POST")
            {
                using (Stream stream = IndexPage)
                using (StreamReader reader = new StreamReader(stream))
                {
                    string result = reader.ReadToEnd();
                    w.ContentType = "text/html";
                    w.ContentLength64 = result.Length;
                    w.OutputStream.Write(Encoding.ASCII.GetBytes(result), 0, result.Length);
                }
                return;
            }
            try
            {
                w.ContentType = "application/octet-stream";
                w.AddHeader("cho-protocol", Program.ProtocolVersion.ToString());

                Presence presence = Presences.GetPresence(r.Headers["osu-token"]);
                if (presence == null)
                {
                    using (StreamReader reader = new StreamReader(r.InputStream, r.ContentEncoding))
                    {
                        presence = new Presence();
                        Handlers.RunHandlers(Handlers.GetHandlers(HandlerTypes.Login),
                            presence, LoginRequest.Parse(reader.ReadToEnd()), w);
                        w.AddHeader("cho-token", presence.Token);
                        WriteStream(presence.OutputStream, w);
                    }
                    return;
                }
            }
            catch (LoginException ex)
            {
                if (ex == LoginExceptions.INVALID_LOGIN_DATA)
                {
                    w.ContentType = "text/html";
                    byte[] outBuff = new byte[] { 0x4e, 0x6f, 0x21 }; // No!
                    w.ContentLength64 = outBuff.Length;
                    w.OutputStream.Write(outBuff, 0, outBuff.Length);
                    w.Close();
                }
            }
            catch (Exception ex) { Console.WriteLine(ex); }
        }
        private void RunServer()
        {
            HttpListener listener = new HttpListener();
            if (!HttpListener.IsSupported)
            {
                Console.WriteLine("Windows XP SP2 or Server 2003 is required to use the HttpListener class.");
                Environment.Exit(1);
            }

            // Add IndexPage.
            listener.Prefixes.Add($"http://+:{Port}/");
            listener.Start();
            Console.WriteLine($"Server is now listening at port {Port}");
            while (!ShouldStop)
            {
                HttpListenerContext context = listener.GetContext();
                pool.QueueWorkItem(Root, context.Request, context.Response);
            }
            pool.Shutdown(true, 5000);
            listener.Stop();
        }
        private void WriteStream(Stream s, HttpListenerResponse w)
        {
            w.ContentLength64 = s.Length;
            s.CopyTo(w.OutputStream);
            s.Flush();
            w.Close();
        }
    }
}
