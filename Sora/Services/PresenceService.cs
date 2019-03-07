using System.Collections.Generic;
using System.Linq;
using System.Threading;
using EventManager.Enums;
using JetBrains.Annotations;
using Sora.Enums;
using Sora.EventArgs;
using Sora.Objects;

namespace Sora.Services
{
    public class PresenceService
    {
        private readonly ChannelService _cs;
        private readonly EventManager.EventManager _ev;
        
        private readonly Dictionary<string, Presence> _presences = new Dictionary<string, Presence>();

        public IEnumerable<Presence> AllPresences => _presences.Select(x => x.Value);

        public PresenceService(ChannelService cs, EventManager.EventManager ev)
        {
            _cs = cs;
            _ev = ev;
        }
        
        public Presence GetPresence(string token)
        {
            return _presences.TryGetValue(token, out Presence pr) ? pr : null;
        }

        public Presence GetPresence(int userid)
        {
            foreach (KeyValuePair<string, Presence> presence in _presences)
                if (presence.Value.User.Id == userid)
                    return presence.Value;

            return null;
        }

        [UsedImplicitly]
        public IEnumerable<int> GetUserIds()
        {
            return _presences.Select(x => x.Value.User.Id);
        }

        public IEnumerable<int> GetUserIds(Presence pr)
        {
            return _presences
                   .Where(x => x.Value.Token != pr.Token)
                   .Select(z => z.Value.User.Id);
        }

        public void BeginPresence(Presence presence)
        {
            // TODO: Add total playtime.
            //presence.BeginSeason = DateTime.UtcNow;
            if (presence == null) return;
            presence.LastRequest.Start();
            _presences.Add(presence.Token, presence);
            _cs.AddChannel(new Channel(presence.User.Username, "", null, presence));
        }

        public void EndPresence(Presence pr, bool forceful)
        {
            if (forceful && _presences.ContainsKey(pr.Token))
            {
                _cs.RemoveChannel(pr.PrivateChannel);

                foreach (PacketStream str in pr.JoinedStreams)
                    str.Left(pr);
                
                _ev.RunEvent(EventType.BanchoExit, new BanchoExitArgs { pr = pr, err = ErrorStates.Ok });
                _ev.RunEvent(EventType.BanchoLobbyPart, new BanchoLobbyPartArgs{ pr = pr});
                _ev.RunEvent(EventType.BanchoMatchPart, new BanchoMatchPartArgs{ pr = pr});
                _ev.RunEvent(EventType.BanchoStopSpectating, new BanchoStopSpectatingArgs { pr = pr });

                pr.Stream.Close();
                pr.LastRequest.Stop();
                
                _presences.Remove(pr.Token);
                return;
            }

            pr.IsLastRequest = true;
        }

        public void TimeoutCheck()
        {
            new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        foreach (KeyValuePair<string, Presence> pr in _presences)
                            if (pr.Value.TimeoutCheck())
                                EndPresence(pr.Value, true);

                        if (_presences == null) break;
                    }
                    catch
                    {
                        // Do not EVER let the TimeoutCheck Crash. else we have a Memory Leak.
                    }

                    Thread.Sleep(1000); // wait a second. we don't want high cpu usage.
                }
            }).Start();
        }
       
    }
}