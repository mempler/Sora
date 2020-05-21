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
    public class OnFriendAddEvent
    {
        private readonly SoraDbContext _ctx;

        public OnFriendAddEvent(SoraDbContext ctx) => _ctx = ctx;

        [Event(EventType.BanchoFriendAdd)]
        public async Task OnFriendAdd(BanchoFriendAddArgs args)
        {
            var u = await DbUser.GetDbUser(_ctx, args.FriendId);

            if (u != null)
                Logger.Info(
                    "%#F94848%" + args.Pr.User.UserName,
                    "%#B342F4%(", args.Pr.User.Id, "%#B342F4%)",
                    "%#FFFFFF%added",
                    "%#F94848%" + u.UserName,
                    "%#B342F4%(", u.Id, "%#B342F4%)%#FFFFFF% as a Friend!"
                );

            await DbFriend.AddFriend(_ctx, args.Pr.User.Id, args.FriendId);
            await _ctx.SaveChangesAsync();
        }
    }
}
