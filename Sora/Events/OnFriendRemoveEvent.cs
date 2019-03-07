using EventManager.Attributes;
using Shared.Models;
using Shared.Services;
using Sora.EventArgs;

namespace Sora.Events
{
    [EventClass]
    public class OnFriendRemoveEvent
    {
        private readonly Database _db;

        public OnFriendRemoveEvent(Database db)
        {
            _db = db;
        }

        public void OnFriendAdd(BanchoFriendRemoveArgs args)
        {
            Friends.RemoveFriend(_db, args.pr.User.Id, args.FriendId);
        }
    }
}