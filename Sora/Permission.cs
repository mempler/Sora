using System.Collections.Generic;
using System.Linq;

namespace Sora
{
    public class Permission
    {
        public const string DEFAULT = "none";
        
        public const string CHAT_MOD_SILENCE = "cmod.silence";
        
        public const string ADMIN_RESTRICT = "admin.restrict";
        public const string ADMIN_KICK = "admin.akick";
        public const string ADMIN_CHANNEL = "admin.channel";
        
        public const string COLOR_ORANGE = "color.orange";
        public const string COLOR_RED = "color.red";
        public const string COLOR_BLUE = "color.blue";

        public static string GROUP_DONATOR = $"{DEFAULT}, {COLOR_ORANGE}";
        public static string GROUP_CHAT_MOD = $"{GROUP_DONATOR}, {COLOR_RED}, {CHAT_MOD_SILENCE}";
        public static string GROUP_ADMIN = $"{GROUP_CHAT_MOD}, {ADMIN_RESTRICT}, {ADMIN_KICK}, {ADMIN_CHANNEL}";
        public static string GROUP_DEVELOPER = $"{GROUP_ADMIN}, {COLOR_BLUE}";
        
        public readonly List<string> Perms;

        public Permission()
        {
            Perms = new List<string>();
        }

        public static Permission From(string perm) => new Permission() + perm.Split(", ");

        public void Add(string perm) => Perms.Add(perm);
        public void Add(IEnumerable<string> perm) => Perms.AddRange(perm);
        public static Permission operator +(Permission instance, string perm)
        {
            instance.Add(perm);
            return instance;
        }
        public static Permission operator +(Permission instance, Permission otherInstance)
        {
            instance.Add(otherInstance.Perms);
            return instance;
        }
        public static Permission operator +(Permission instance, IEnumerable<string> perms)
        {
            instance.Add(perms);
            return instance;
        }

        public void Remove(string perm) => Perms.Remove(perm);
        public void Remove(IEnumerable<string> perm) => Perms.RemoveAll(x => perm.Any(p => x == p));
        public static Permission operator -(Permission instance, string perm)
        {
            instance.Remove(perm);
            return instance;
        }

        public bool HasPermission(string perm) => Perms.Any(p => perm == p);
        public bool this[string perm] => Perms.Any(p => perm == p);
        
        public static bool operator ==(Permission instance, string perm) => instance?.HasPermission(perm) ?? true;
        public static bool operator !=(Permission instance, string perm) => !(instance?.HasPermission(perm) ?? true);

        public static bool operator ==(Permission instance, Permission other)
            => instance?.HasPermission(other?.ToString() ?? "") ?? true;
        public static bool operator !=(Permission instance, Permission other)
            => !(instance?.HasPermission(other?.ToString() ?? "") ?? true);

        private bool Equals(Permission other) => Equals(ToString(), other.ToString());

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((Permission) obj);
        }

        public override int GetHashCode() => Perms != null ? Perms.GetHashCode() : 0;

        public override string ToString() => string.Join(", ", Perms.ToArray());
    }
}