using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;


#nullable enable

namespace Sora.Database.Models
{
    [Table("Friends")]
    [UsedImplicitly]
    public class DbFriend
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }
        
        [Required]
        [ForeignKey(nameof(UserId))]
        public DbUser User { get; set; }

        [Required]
        public int FriendId { get; set; }
        
        [Required]
        [UsedImplicitly]
        [ForeignKey(nameof(FriendId))]
        public DbUser Friend { get; set; }

        public static IEnumerable<int> GetFriends(SoraDbContext ctx, int userId)
            => ctx
                .Friends
                .Where(t => t.UserId == userId)
                .Select(x => x.FriendId).ToList();

        public static async Task AddFriend(SoraDbContext ctx, int userId, int friendId)
        {
            await ctx.Friends.AddAsync(new DbFriend {UserId = userId, FriendId = friendId});

            await ctx.SaveChangesAsync();
        }

        public static async Task RemoveFriend(SoraDbContext ctx, int userId, int friendId)
        {
            ctx.RemoveRange(ctx.Friends.Where(x => x.UserId == userId && x.FriendId == friendId));
            
            await ctx.SaveChangesAsync();
        }
    }
}
