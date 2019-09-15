using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Markdig.Extensions.Tables;

namespace Ripple.MergeTool.Database.Model
{
    [Table("users_relationships")]
    public class RippleFriends
    {
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        [Required]
        public int user1 { get; set; }

        [Required]
        public int user2 { get; set; }
    }
}
