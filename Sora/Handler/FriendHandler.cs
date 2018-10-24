using Shared.Database.Models;
using Shared.Enums;
using Shared.Handlers;
using Sora.Objects;

namespace Sora.Handler
{
    public class FriendHandler
    {
        [Handler(HandlerTypes.ClientFriendAdd)]
        public void OnFriendAdd(Presence pr, int friendId)
        {
            Friends.AddFriend(pr.User.Id, friendId);
        }

        [Handler(HandlerTypes.ClientFriendRemove)]
        public void OnFriendRemove(Presence pr, int friendId)
        {
            Friends.RemoveFriend(pr.User.Id, friendId);
        }
    }
}