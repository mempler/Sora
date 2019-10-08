using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Sora.Database.Models
{
    [Flags]
    public enum OAuthClientFlags
    {
        None = 0,
        PasswordAuth = 1<<1
    }
    
    [Table("OAuthClients")]
    public class DBOAuthClient
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public int OwnerId { get; set; }

        [Required]
        [ForeignKey(nameof(OwnerId))]
        public DBUser Owner { get; set; }
        
        public string Secret { get; set; }
        public OAuthClientFlags Flags { get; set; }
        public bool Disabled { get; set; }

        public static Task<DBOAuthClient> GetClient(SoraDbContextFactory factory, string id)
        {
            var gid = new Guid(id);
            return factory.Get().OAuthClients.FirstOrDefaultAsync(x => x.Id == gid);
        }
    }
}