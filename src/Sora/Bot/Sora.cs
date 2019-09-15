using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sora.Attributes;
using Sora.Bot.Commands;
using Sora.Database;
using Sora.Database.Models;
using Sora.Enums;
using Sora.EventArgs;
using Sora.Helpers;
using Sora.Objects;
using Sora.Packets.Client;
using Sora.Packets.Server;
using Sora.Services;

namespace Sora.Bot
{
    public struct SoraCommand
    {
        public delegate bool SoraCommandExecution(Presence executor, string[] args);

        public string Command;
        public string Description;
        public List<Argument> Args;
        public int ExpectedArgs;
        public Permission RequiredPermission;
        public SoraCommandExecution Callback;
    }

    [EventClass]
    public class Sora
    {
        private readonly List<SoraCommand> _commands = new List<SoraCommand>();
        private readonly ChannelService _cs;
        private readonly EventManager _ev;
        private readonly SoraDbContextFactory _factory;
        private readonly object _mut = new object();
        private readonly IServiceProvider _provider;
        private readonly PresenceService _ps;
        private readonly PacketStreamService _pss;
        private MultiplayerService _ms;

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

        private Presence _botPresence { get; set; }

        public void RegisterCommandClass<T>()
            where T : ISoraCommand
        {
            var t = typeof(T);
            var tArgs = (from cInfo in t.GetConstructors()
                         from pInfo in cInfo.GetParameters()
                         select _provider.GetService(pInfo.ParameterType)).ToArray();

            if (tArgs.Any(x => x == null))
                throw new Exception("Could not find Dependency, are you sure you registered the Dependency?");

            var cls = (ISoraCommand) Activator.CreateInstance(t, tArgs);
            lock (_mut)
            {
                _commands.Add(
                    new SoraCommand
                    {
                        Args = cls.Args,
                        Command = cls.Command,
                        Description = cls.Description,
                        ExpectedArgs = cls.ExpectedArgs,
                        RequiredPermission = cls.RequiredPermission,
                        Callback = cls.Execute
                    }
                );
            }
        }

        public Task RunAsync()
        {
            _botPresence = new Presence(_cs)
            {
                User = Users.GetUser(_factory, 100)
            };

            _botPresence["STATUS"] = new UserStatus
            {
                Status = Status.Watching,
                Playmode = PlayMode.Osu,
                BeatmapChecksum = "nothing",
                BeatmapId = 0,
                StatusText = "over you!",
                CurrentMods = Mod.TouchDevice
            };
            _botPresence["BOT"] = true;
            _botPresence["IRC"] = true;
            _botPresence["LB_STD"] = LeaderboardStd.GetLeaderboard(_factory.Get(), 100);
            _botPresence["LB_RX"] = LeaderboardRx.GetLeaderboard(_factory.Get(), 100);
            
            _botPresence["TIMEZONE"] = 0;
            _botPresence["BLOCK_NON_FRIENDS_DM"] = false;
            _botPresence["LON"] = 0d;
            _botPresence["LAT"] = 0d;
            _botPresence["COUNTRYID"] = CountryIds.XX;

            var stream = _pss.GetStream("main");
            if (stream != null)
            {
                stream.Broadcast(new PresenceSingle(_botPresence.User.Id));
                stream.Broadcast(new UserPresence(_botPresence));
                stream.Broadcast(new HandleUpdate(_botPresence));
            }

            _ps.BeginPresence(_botPresence);

            Logger.Info("Hey, I'm olSora! I'm a bot and i say Hello World!");

            return Task.CompletedTask;
        }

        public void SoraCommand(string Command, string Description, List<Argument> args,
            SoraCommand.SoraCommandExecution cb)
        {
            lock (_mut)
            {
                _commands.Add(
                    new SoraCommand {Command = Command, Description = Description, Args = args, Callback = cb}
                );
            }
        }

        public IEnumerable<SoraCommand> GetCommands(string Command)
        {
            lock (_mut)
            {
                return _commands.Where(z => z.Command == Command.Split(" ")[0]);
            }
        }

        public async void SendMessage(string msg, string channelTarget, bool isPrivate)
        {
            if (_cs.GetChannel(channelTarget) == null)
                return;

            if (_botPresence == null)
                _botPresence = _ps.GetPresence(100);

            if (!isPrivate)
                await _ev.RunEvent(
                    EventType.BanchoSendIrcMessage,
                    new BanchoSendIRCMessageArgs
                    {
                        Message = new MessageStruct
                        {
                            Message = msg,
                            Username = _botPresence.User.Username,
                            ChannelTarget = channelTarget,
                            SenderId = _botPresence.User.Id
                        },
                        pr = _botPresence
                    }
                );
            else
                await _ev.RunEvent(
                    EventType.BanchoSendIrcMessagePrivate,
                    new BanchoSendIRCMessageArgs
                    {
                        Message = new MessageStruct
                        {
                            Message = msg,
                            Username = _botPresence.User.Username,
                            ChannelTarget = channelTarget,
                            SenderId = _botPresence.User.Id
                        },
                        pr = _botPresence
                    }
                );
        }

        [Event(EventType.BanchoSendIrcMessage)]
        public void OnPublicMessageEvent(BanchoSendIRCMessageArgs args)
        {
            if (args.pr == _botPresence)
                return;

            if (args.Message.Message.StartsWith("!"))
            {
                if (_cs.GetChannel(args.Message.ChannelTarget) == null)
                    return;

                var cmds = GetCommands(args.Message.Message.TrimStart('!'));
                foreach (var cmd in cmds)
                {
                    if (args.pr.User.Permissions != cmd.RequiredPermission)
                        continue;

                    var l = args.Message.Message.TrimStart('!').Split(" ")[1..].ToList();
                    if (l.Count < cmd.ExpectedArgs)
                    {
                        var aList = "\t<";

                        var i = 0;
                        foreach (var a in cmd.Args)
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

                        SendMessage(
                            $"Insufficient amount of Arguments!\nUsage:\n     !{cmd.Command} {aList}",
                            args.Message.ChannelTarget, false
                        );
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
            if (args.pr == _botPresence)
                return;
        }
    }
}
