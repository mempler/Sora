using System;
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
        private readonly ChannelService _cs;
        private readonly PacketStreamService _pss;
        private ConnectionMultiplexer Redis;
        private Thread UpdateLoopThread;

        private bool Stop;

        public PluginEntry(PresenceService ps, Database db, ChannelService cs, PacketStreamService pss)
        {
            _ps = ps;
            _db = db;
            _cs = cs;
            _pss = pss;
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
                            if (pr == null)
                                break;

                            Database _ddb = new Database();
                            
                            pr.LeaderboardRx = LeaderboardRx.GetLeaderboard(_ddb, pr.User);
                            pr.LeaderboardStd = LeaderboardStd.GetLeaderboard(_ddb, pr.User);
                            
                            pr.Relax = Value.Split("|")[1] == "True";
                            if (pr.Relax)
                                pr.Rank = pr.LeaderboardRx.GetPosition(_ddb, pr.Status.Playmode);
                            else
                                pr.Rank = pr.LeaderboardStd.GetPosition(_ddb, pr.Status.Playmode);
                            
                            pr.Write(new HandleUpdate(pr));
                            
                            _ddb.Dispose();
                        } break;

                        case "IsRelaxing":
                        {
                            Presence pr = _ps.GetPresence(Convert.ToInt32(Value.Split("|")[0]));
                            if (pr == null)
                                break;
                            
                            pr.Relax = Value.Split("|")[1] == "True";
                            pr.Write(new HandleUpdate(pr));
                        } break;

                        case "SendMessage":
                        {
                            Presence pr = _ps.GetPresence(Convert.ToInt32(Value.Split("|")[0]));
                            if (pr == null)
                                break;
                            
                            string Channel = Value.Split("|")[1];
                            string Message = Value.Split("|")[2];

                            Channel c;
                            if ((c = _cs.GetChannel(Channel)) == null)
                                break;
                            
                            c.WriteMessage(pr, Message, true);
                            
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