using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sora.Attributes;
using Sora.Bot.Commands;
using Sora.Database;
using Sora.Database.Models;
using Sora.Enums;
using Sora.EventArgs.BanchoEventArgs;
using Sora.Framework;
using Sora.Framework.Enums;
using Sora.Framework.Objects;
using Sora.Framework.Packets.Server;
using Sora.Framework.Utilities;
using Sora.Services;

namespace Sora.Bot
{
    public struct Argument
    {
        public string ArgName;
    }

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
        private readonly SoraDbContext _ctx;
        private readonly object _mut = new object();
        private readonly IServiceProvider _provider;
        private readonly PresenceService _ps;
        
        private readonly DbUser _dbUser;

        public Sora(SoraDbContext ctx,
            IServiceProvider provider,
            PresenceService ps,
            ChannelService cs,
            EventManager ev
        )
        {
            _provider = provider;
            _ctx = ctx;
            _ps = ps;
            _cs = cs;
            _ev = ev;

            #region DEFAULT COMMANDS

            RegisterCommandClass<RestrictCommand>();
            RegisterCommandClass<DebugCommand>();

            #endregion

            ctx.Migrate();
            
            // this will fail if bot already exists!
            DbUser.RegisterUser(_ctx, Permission.From(Permission.GROUP_ADMIN), "Sora", "bot@gigamons.de",
                Crypto.RandomString(32), false, PasswordVersion.V2, 100);
            
            _dbUser = DbUser.GetDbUser(ctx, 100).Result;
        }

        private static Presence BotPresence { get; set; }

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
            BotPresence = new Presence(_dbUser.ToUser())
            {
                Status = new UserStatus
                {
                    Status = Status.Watching,
                    Playmode = PlayMode.Osu,
                    BeatmapChecksum = "nothing",
                    BeatmapId = 0,
                    StatusText = "over you!",
                    CurrentMods = Mod.TouchDevice
                },
                Info = new UserInformation
                {
                    Latitude = 0,
                    Longitude = 0,
                    ClientPermission = LoginPermissions.User |
                                       LoginPermissions.Administrator |
                                       LoginPermissions.Moderator |
                                       LoginPermissions.Supporter|
                                       LoginPermissions.TorneyStaff |
                                       LoginPermissions.BAT,
                    CountryId = CountryId.XX,
                    RankingPosition = 0,
                    TimeZone = 0
                },
                ["BOT"] = true,
                ["IRC"] = true
            };
            
            _ps.Push(new PresenceSingle(BotPresence.User.Id));
            _ps.Push(new UserPresence(BotPresence));
            _ps.Push(new HandleUpdate(BotPresence));

            _ps.Join(BotPresence);

            Logger.Info("Hey, I'm Sora! I'm a bot and i say Hello World!");

            return Task.CompletedTask;
        }

        public void SoraCommand(string command, string description, List<Argument> args,
            SoraCommand.SoraCommandExecution cb)
        {
            lock (_mut)
            {
                _commands.Add(
                    new SoraCommand {Command = command, Description = description, Args = args, Callback = cb}
                );
            }
        }

        public IEnumerable<SoraCommand> GetCommands(string command)
        {
            lock (_mut)
            {
                return _commands.Where(z => z.Command == command.Split(" ")[0]);
            }
        }

        public async void SendMessage(string msg, string channelTarget, bool isPrivate)
        {
            if (!_cs.TryGet(channelTarget, out var _))
                return;
            
            if (!isPrivate)
                await _ev.RunEvent(
                    EventType.BanchoSendIrcMessage,
                    new BanchoSendIrcMessageArgs
                    {
                        Message = new MessageStruct
                        {
                            Message = msg,
                            Username = BotPresence.User.UserName,
                            ChannelTarget = channelTarget,
                            SenderId = BotPresence.User.Id
                        },
                        Pr = BotPresence
                    }
                );
            else
                await _ev.RunEvent(
                    EventType.BanchoSendIrcMessagePrivate,
                    new BanchoSendIrcMessageArgs
                    {
                        Message = new MessageStruct
                        {
                            Message = msg,
                            Username = BotPresence.User.UserName,
                            ChannelTarget = channelTarget,
                            SenderId = BotPresence.User.Id
                        },
                        Pr = BotPresence
                    }
                );
        }

        [Event(EventType.BanchoSendIrcMessage)]
        public void OnPublicMessageEvent(BanchoSendIrcMessageArgs args)
        {
            if (args.Pr == BotPresence)
                return;

            if (!args.Message.Message.StartsWith("!"))
                return;
            
            if (!_cs.TryGet(args.Message.ChannelTarget, out var _))
                return;

            var cmds = GetCommands(args.Message.Message.TrimStart('!'));
            foreach (var cmd in cmds)
            {
                if (args.Pr.User.Permissions != cmd.RequiredPermission)
                    continue;

                var l = args.Message.Message.TrimStart('!').Split(" ").ToList();
                l.RemoveAt(0);
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

                if (cmd.Callback(args.Pr, l.ToArray()))
                    break;
            }
        }

        [Event(EventType.BanchoSendIrcMessage)]
        public void OnPrivateMessageEvent(BanchoSendIrcMessageArgs args)
        {
            if (args.Pr == BotPresence)
                return;
        }
    }
}
