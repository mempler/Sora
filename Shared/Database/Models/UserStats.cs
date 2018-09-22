using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Shared.Database.Models
{
    public class UserStats
    {
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [DefaultValue(0)]
        public byte CountryId { get; set; }

        
        public static UserStats GetUserStats(int userId)
        {
            using (var db = new SoraContext())
            {
                var result = db.UserStats.Where(t => t.Id == userId).Select(e => e).FirstOrDefault();
                return result;
            }
        }

        public static UserStats GetUserStats(Users user) => GetUserStats(user.Id);
    }
}
