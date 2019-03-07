using System.Collections.Generic;
using System.Linq;
using Sora.Objects;

namespace Sora.Services
{
    public class MultiplayerService
    {
        private long _matches;
        public Dictionary<long, MultiplayerRoom> Rooms = new Dictionary<long, MultiplayerRoom>();

        public IEnumerable<MultiplayerRoom> GetRooms()
        {
            return Rooms.Select(x => x.Value);
        }

        public MultiplayerRoom GetRoom(long matchId)
        {
            return Rooms.ContainsKey(matchId) ? Rooms[matchId] : null;
        }

        public long Add(MultiplayerRoom room)
        {
            room.MatchId = _matches++;
            Rooms.Add(room.MatchId, room);
            return room.MatchId;
        }

        public void Remove(long matchId)
        {
            if (!Rooms.ContainsKey(matchId)) return;
            Rooms.Remove(matchId);
        }
    }
}