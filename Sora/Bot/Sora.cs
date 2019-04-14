using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventManager.Attributes;
using EventManager.Enums;
using Shared.Enums;
using Shared.Helpers;
using Shared.Models;
using Shared.Services;
using Sora.Enums;
using Sora.EventArgs;
using Sora.Events;
using Sora.Objects;
using Sora.Packets.Client;
using Sora.Packets.Server;
using Sora.Services;

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
        private EventManager.EventManager _ev;
        private PacketStreamService _pss;
        private MultiplayerService _ms;
        private PresenceService _ps;
        private ChannelService _cs;
        private Database _db;
        private object _mut = new object();
              
        private Presence _botPresence;

        public Sora(Database db, MultiplayerService ms, PresenceService ps, ChannelService cs, EventManager.EventManager ev, PacketStreamService pss)
        {
            _db = db;
            _ms = ms;
            _ps = ps;
            _cs = cs;
            _ev = ev;
            _pss = pss;

            _botPresence = new Presence(cs)
            {
                User = Users.GetUser(_db, 100),
                Status = new UserStatus
                {
                    Status          = Status.Watching,
                    Playmode        = PlayMode.Osu,
                    BeatmapChecksum = "nothing",
                    BeatmapId       = 0,
                    StatusText      = "over you!",
                    CurrentMods     = (uint) Mod.TouchDevice
                },
                LeaderboardRx    = LeaderboardRx.GetLeaderboard(_db, 100),
                LeaderboardStd   = LeaderboardStd.GetLeaderboard(_db, 100),
                Timezone         = 0,
                BlockNonFriendDm = false,
                Lon              = 0d,
                Lat              = 0d,
                BotPresence      = true
            };

            PacketStream stream = _pss.GetStream("main");
            if (stream == null) return;

            stream.Broadcast(new PresenceSingle(_botPresence.User.Id));
            stream.Broadcast(new UserPresence(_botPresence));
            stream.Broadcast(new HandleUpdate(_botPresence));
        }

        public Task RunAsync()
        {
            Logger.Info("Hey, I'm Sora! I'm a bot and i say Hello World!");
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