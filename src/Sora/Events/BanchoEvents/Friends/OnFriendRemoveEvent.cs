using System.Threading.Tasks;
using Sora.Attributes;
using Sora.Database;
using Sora.Database.Models;
using Sora.Enums;
using Sora.EventArgs.BanchoEventArgs;
using Sora.Framework.Utilities;

namespace Sora.Events.BanchoEvents.Friends
{
    [EventClass]
    public class OnFriendRemoveEvent
    {
        private readonly SoraDbContextFactory _factory;

        public OnFriendRemoveEvent(SoraDbContextFactory factory) => _factory = factory;

        [Event(EventType.BanchoFriendRemove)]
        public async Task OnFriendRemove(BanchoFriendRemoveArgs args)
        {
            var u = await DbUser.GetDbUser(_factory, args.FriendId);

            if (u != null)
                Logger.Info(
                    "%#F94848%" + args.Pr.User.UserName,
                    "%#B342F4%(", args.Pr.User.Id, "%#B342F4%)",
                    "%#FFFFFF%removed",
                    "%#F94848%" + u.UserName,
                    "%#B342F4%(", u.Id, "%#B342F4%)%#FFFFFF% as Friend!"
                );

            DbFriend.RemoveFriend(_factory, args.Pr.User.Id, args.FriendId);
        }
    }
}
