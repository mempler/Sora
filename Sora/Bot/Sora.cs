using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sora.Attributes;
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
        public delegate bool SoraCommandExecution(string[] args);

        public string Command;
        public string Description;
        public List<Argument> Args;
        public Privileges RequiredPrivileges;
        public SoraCommandExecution Callback;
    }
    
    [EventClass]
    public class Sora
    {
        private List<SoraCommand> _commands = new List<SoraCommand>();
        private EventManager _ev;
        private PacketStreamService _pss;
        private MultiplayerService _ms;
        private PresenceService _ps;
        private ChannelService _cs;
        private SoraDbContextFactory _factory;
        private object _mut = new object();
              
        private Presence _botPresence { get; set; }

        public Sora(SoraDbContextFactory factory, MultiplayerService ms, PresenceService ps, ChannelService cs, EventManager ev, PacketStreamService pss)
        {
            _factory = factory;
            _ms = ms;
            _ps = ps;
            _cs = cs;
            _ev = ev;
            _pss = pss;
        }

        public Task RunAsync()
        {
            Logger.Info("Hey, I'm Sora! I'm a bot and i say Hello World!");
            
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
                Channel c;
                if ((c = _cs.GetChannel(args.Message.ChannelTarget)) == null)
                    return;

                IEnumerable<SoraCommand> cmds = GetCommands(args.Message.Message.TrimStart('!'));
            }
        }

        [Event(EventType.BanchoSendIrcMessage)]
        public void OnPrivateMessageEvent(BanchoSendIRCMessageArgs args)
        {
            if (args.pr == _botPresence) return;
        }
    }
}