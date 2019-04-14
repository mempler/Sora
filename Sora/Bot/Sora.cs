using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using EventManager.Attributes;
using EventManager.Enums;
using Shared.Enums;
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
        private MultiplayerService _ms;
        private PresenceService _ps;
        private ChannelService _cs;
        private Database _db;
        private object _mut;
              
        private Presence _botPresence;

        public Sora(Database db, MultiplayerService ms, PresenceService ps, ChannelService cs, EventManager.EventManager ev)
        {
            _db = db;
            _ms = ms;
            _ps = ps;
            _cs = cs;
            _ev = ev;
            
            _botPresence = new Presence(cs);

            _botPresence.User = Users.GetUser(_db, 101);
            _botPresence.Status = new UserStatus
            {
                Status = Status.Watching,
                Playmode = PlayMode.Osu,
                BeatmapChecksum = "",
                BeatmapId = 0,
                StatusText = "Over you!",
                CurrentMods = 0
            };
            _botPresence.LeaderboardRx = LeaderboardRx.GetLeaderboard(_db, 101);
            _botPresence.LeaderboardStd = LeaderboardStd.GetLeaderboard(_db, 101);

            _botPresence.Timezone         = 0;
            _botPresence.BlockNonFriendDm = false;

            _botPresence.Lon  = 0d;
            _botPresence.Lat  = 0d;
            
            _ps.BeginPresence(_botPresence);
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