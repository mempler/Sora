using Shared.Database.Models;
using Shared.Enums;
using Shared.Handlers;
using Sora.Objects;

namespace Sora.Handler
{
    public class FriendHandler
    {
        [Handler(HandlerTypes.ClientFriendAdd)]
        public void OnFriendAdd(Presence pr, int FriendId)
        {
            Friends.AddFriend(pr.User.Id, FriendId);
        }

        [Handler(HandlerTypes.ClientFriendRemove)]
        public void OnFriendRemove(Presence pr, int FriendId)
        {
            Friends.RemoveFriend(pr.User.Id, FriendId);
        }
    }
}