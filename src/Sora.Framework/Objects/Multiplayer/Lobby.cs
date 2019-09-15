using System;
using System.Collections.Generic;
using System.Linq;

namespace Sora.Framework.Objects.Multiplayer
{
    public class Lobby : PresenceKeeper, IAsyncKeeper<long, Match>
    {
        public static Lobby Self { get; } = new Lobby();

        protected readonly Dictionary<long, Match> Matches =
            new Dictionary<long, Match>();

        public IEnumerable<Match> Rooms => Matches.Select(m => m.Value);

        public void Push(Match val)
        {
            try
            {
                RWL.AcquireReaderLock(1000); // this is already too long but who cares ?

                val.MatchId = Matches.Count;
                Matches.Add(val.MatchId, val);
            }
            finally
            {
                RWL.ReleaseReaderLock();
            }
        }

        public void Push(long key, Match val)
        {
            throw new NotImplementedException();
        }

        public void Pop(long key)
        {
            try
            {
                RWL.AcquireReaderLock(1000); // this is already too long but who cares ?

                Matches.Remove(key);
            }
            finally
            {
                RWL.ReleaseReaderLock();
            }
        }

        public void Pop(Match key)
        {
            Pop(key.MatchId);
        }

        public bool TryGet(long key, out Match val)
        {
            try
            {
                RWL.AcquireReaderLock(500);

                return Matches.TryGetValue(key, out val);
            }
            finally
            {
                RWL.ReleaseReaderLock();
            }
        }
    }
}