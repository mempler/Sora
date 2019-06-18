using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Ripple.MergeTool.Enums;

namespace Ripple.MergeTool.Database.Model
{
    [Table("users")]
    public class RippleUser
    {
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        
        [Required]
        public string username { get; set; }

        [Required]
        public string password_md5 { get; set; }

        [Required]
        public string email { get; set; }

        [Required]
        public RipplePrivileges privileges { get; set; }
    }
}
