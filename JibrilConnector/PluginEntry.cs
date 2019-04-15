using System;
using System.IO;
using System.Threading;
using EventManager;
using Shared.Helpers;
using Shared.Models;
using Shared.Services;
using Sora.Helpers;
using Sora.Objects;
using Sora.Packets.Server;
using Sora.Services;
using StackExchange.Redis;

namespace JibrilConnector
{
    public class PluginEntry : Plugin
    {
        private readonly PresenceService _ps;
        private readonly Database _db;
        private ConnectionMultiplexer Redis;
        private Thread UpdateLoopThread;

        private bool Stop;

        public PluginEntry(PresenceService ps, Database db)
        {
            _ps = ps;
            _db = db;
        }
        
        public override void OnEnable()
        {
            Logger.Info("Startup JibrilConnector");
            
            Redis = ConnectionMultiplexer.Connect(ConfigUtil.ReadConfig<Config>().Redis.Hostname ?? "localhost");
            UpdateLoopThread = new Thread(UpdateThread);
            
            Logger.Info("Begin Thread of JibrilConnector");
            UpdateLoopThread.Start();
        }

        public override void OnDisable()
        {
            Logger.Info("Shutdown JibrilConnector");

            Stop = true;
            UpdateLoopThread.Join();
        }

        private void UpdateThread()
        {
            while (!Stop)
            {
                HashEntry[] x = Redis.GetDatabase().HashGetAll("jibril:connector:events");

                foreach (HashEntry z in x)
                {
                    string Key = z.Value.ToString().Split(",")[0];
                    string Value = z.Value.ToString().Split(",")[1];
                    
                    Logger.Debug($"Incomming Request: {Key} Value {Value}");
                    switch (Key)
                    {
                        case "SubmitScore":
                        {
                            Presence pr = _ps.GetPresence(Convert.ToInt32(Value.Split("|")[0]));
                            if (Value.Split("|")[1] == "True")
                                pr.LeaderboardRx = LeaderboardRx.GetLeaderboard(_db, pr.User);
                            else
                                pr.LeaderboardStd = LeaderboardStd.GetLeaderboard(_db, pr.User);
                            
                            pr.Relax = Value.Split("|")[1] == "True";
                            
                            pr.Write(new HandleUpdate(pr));
                        } break;

                        case "IsRelaxing":
                        {
                            Presence pr = _ps.GetPresence(Convert.ToInt32(Value.Split("|")[0]));
                            pr.Relax = Value.Split("|")[1] == "True";
                            pr.Write(new HandleUpdate(pr));
                        } break;
                        default:
                            Logger.Info($"Unknown Key: {Key} Value: {Value}");
                            break;
                    }
                    
                    // We don't need it anymore. be GONE THOT!
                    Redis.GetDatabase().HashDelete("jibril:connector:events", z.Name);
                }
                
                Thread.Sleep(200);
            }
        }
    }
}