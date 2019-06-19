using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Sora.Attributes;
using Sora.Bot.Commands;
using Sora.Database;
using Sora.Enums;
using Sora.EventArgs;
using Sora.Objects;
using Sora.Packets.Client;
using Sora.Packets.Server;
using Sora.Services;
using LeaderboardRx = Sora.Database.Models.LeaderboardRx;
using LeaderboardStd = Sora.Database.Models.LeaderboardStd;
using Logger = Sora.Helpers.Logger;
using Mod = Sora.Enums.Mod;
using PlayMode = Sora.Enums.PlayMode;
using Privileges = Sora.Enums.Privileges;
using Users = Sora.Database.Models.Users;

namespace Sora.Bot
{
    public struct SoraCommand
    {
        public delegate bool SoraCommandExecution(Presence executor, string[] args);

        public string Command;
        public string Description;
        public List<Argument> Args;
        public int ExpectedArgs;
        public Privileges RequiredPrivileges;
        public SoraCommandExecution Callback;
    }
    
    [EventClass]
    public class Sora
    {
        private List<SoraCommand> _commands = new List<SoraCommand>();
        private EventManager _ev;
        private PacketStreamService _pss;
        private readonly IServiceProvider _provider;
        private MultiplayerService _ms;
        private PresenceService _ps;
        private ChannelService _cs;
        private SoraDbContextFactory _factory;
        private object _mut = new object();
              
        private Presence _botPresence { get; set; }
        
        public Sora(SoraDbContextFactory factory,
                    IServiceProvider provider,
                    PacketStreamService pss,
                    MultiplayerService ms,
                    PresenceService ps,
                    ChannelService cs,
                    EventManager ev
            )
        {
            _provider = provider;
            _factory = factory;
            _pss = pss;
            _ms = ms;
            _ps = ps;
            _cs = cs;
            _ev = ev;

            #region DEFAULT COMMANDS

            RegisterCommandClass<RestrictCommand>();
            RegisterCommandClass<DebugCommand>();

            #endregion
        }

        public void RegisterCommandClass<T>()
            where T : ISoraCommand
        {
            Type t = typeof(T);
            object[] tArgs = (from cInfo in t.GetConstructors()
                              from pInfo in cInfo.GetParameters()
                              select _provider.GetService(pInfo.ParameterType)).ToArray();

            if (tArgs.Any(x => x == null))
                throw new Exception("Could not find Dependency, are you sure you registered the Dependency?");

            ISoraCommand cls = (ISoraCommand) Activator.CreateInstance(t, tArgs);
            lock(_mut)
                _commands.Add(new SoraCommand
                {
                    Args = cls.Args,
                    Command = cls.Command,
                    Description = cls.Description,
                    ExpectedArgs = cls.ExpectedArgs,
                    RequiredPrivileges = cls.RequiredPrivileges,
                    Callback = cls.Execute
                });
        }

        public Task RunAsync()
        {
            _botPresence = new Presence(_cs)
            {
                User = Users.GetUser(_factory, 100),
                Status = new UserStatus
                {
                    Status          = Status.Watching,
                    Playmode        = PlayMode.Osu,
                    BeatmapChecksum = "nothing",
                    BeatmapId       = 0,
                    StatusText      = "over you!",
                    CurrentMods     = Mod.TouchDevice
                },
                LeaderboardRx    = LeaderboardRx.GetLeaderboard(_factory, 100),
                LeaderboardStd   = LeaderboardStd.GetLeaderboard(_factory, 100),
                Timezone         = 0,
                BlockNonFriendDm = false,
                Lon              = 0d,
                Lat              = 0d,
                BotPresence      = true
            };

            PacketStream stream = _pss.GetStream("main");
            if (stream != null)
            {
                stream.Broadcast(new PresenceSingle(_botPresence.User.Id));
                stream.Broadcast(new UserPresence(_botPresence));
                stream.Broadcast(new HandleUpdate(_botPresence));
            }

            _ps.BeginPresence(_botPresence);
            
            Logger.Info("Hey, I'm Sora! I'm a bot and i say Hello World!");

            return Task.CompletedTask;
        }
        
        public void SoraCommand(string Command, string Description, List<Argument> args,
                                SoraCommand.SoraCommandExecution cb)
        {
            lock (_mut)
                _commands.Add(new SoraCommand
                {
                    Command     = Command,
                    Description = Description,
                    Args        = args,
                    Callback    = cb
                });
        }

        public IEnumerable<SoraCommand> GetCommands(string Command)
        {
            lock (_mut)
                return _commands.Where(z => z.Command == Command.Split(" ")[0]);
        }

        public async void SendMessage(string msg, string channelTarget, bool isPrivate)
        {
            if (_cs.GetChannel(channelTarget) == null)
                return;

            if (_botPresence == null)
                _botPresence = _ps.GetPresence(100);

            if (!isPrivate)
                await _ev.RunEvent(EventType.BanchoSendIrcMessage, new BanchoSendIRCMessageArgs
                {
                    Message = new MessageStruct
                    {
                        Message       = msg,
                        Username      = _botPresence.User.Username,
                        ChannelTarget = channelTarget,
                        SenderId      = _botPresence.User.Id
                    },
                    pr = _botPresence
                });
            else
                await _ev.RunEvent(EventType.BanchoSendIrcMessagePrivate, new BanchoSendIRCMessageArgs
                {
                    Message = new MessageStruct
                    {
                        Message       = msg,
                        Username      = _botPresence.User.Username,
                        ChannelTarget = channelTarget,
                        SenderId      = _botPresence.User.Id
                    },
                    pr = _botPresence
                });
        }

        [Event(EventType.BanchoSendIrcMessage)]
        public void OnPublicMessageEvent(BanchoSendIRCMessageArgs args)
        {
            if (args.pr == _botPresence) return;
            
            if (args.Message.Message.StartsWith("!"))
            {
                if (_cs.GetChannel(args.Message.ChannelTarget) == null)
                    return;

                IEnumerable<SoraCommand> cmds = GetCommands(args.Message.Message.TrimStart('!'));
                foreach (SoraCommand cmd in cmds)
                {
                    if (!args.pr.User.HasPrivileges(cmd.RequiredPrivileges))
                        continue;
                    
                    List<string> l = args.Message.Message.TrimStart('!').Split(" ")[1..].ToList();
                    if (l.Count < cmd.ExpectedArgs)
                    {
                        string aList = "\t<";

                        int i = 0;
                        foreach (Argument a in cmd.Args)
                        {
                            aList += a.ArgName;
                            if (i >= cmd.ExpectedArgs)
                                aList += "?";
                            aList += ", ";
                            i++;
                        }

                        aList = aList.TrimEnd(cmd.Args.Count < 1 ? '<' : ' ').TrimEnd(',');
                        if (cmd.Args.Count > 0)
                            aList += ">";

                        SendMessage($"Insufficient amount of Arguments!\nUsage:\n     !{cmd.Command} {aList}", args.Message.ChannelTarget, false);
                        break;
                    }

                    if (cmd.Callback(args.pr, l.ToArray()))
                        break;
                }
            }
        }

        [Event(EventType.BanchoSendIrcMessage)]
        public void OnPrivateMessageEvent(BanchoSendIRCMessageArgs args)
        {
            if (args.pr == _botPresence) return;
        }
    }
}