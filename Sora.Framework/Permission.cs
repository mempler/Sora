using System.Collections.Generic;
using System.Linq;

namespace Sora.Framework
{
    public class Permission
    {
        public const string DEFAULT = "none";

        public const string CHAT_MOD_SILENCE = "cmod.silence";

        public const string ADMIN_RESTRICT = "admin.restrict";
        public const string ADMIN_KICK = "admin.akick";
        public const string ADMIN_CHANNEL = "admin.channel";
        public const string ADMIN_CHANNEL_READONLY = "admin.channel.readonly"; // Bypass Readonly

        public const string COLOR_ORANGE = "color.orange";
        public const string COLOR_RED = "color.red";
        public const string COLOR_BLUE = "color.blue";

        public static string GROUP_DONATOR = $"{DEFAULT}, {COLOR_ORANGE}";
        public static string GROUP_CHAT_MOD = $"{GROUP_DONATOR}, {COLOR_RED}, {CHAT_MOD_SILENCE}";
        public static string GROUP_ADMIN = $"{GROUP_CHAT_MOD}, " +
                                           $"{ADMIN_RESTRICT}, " +
                                           $"{ADMIN_KICK}, " +
                                           $"{ADMIN_CHANNEL}, " +
                                           $"{ADMIN_CHANNEL_READONLY}";
        public static string GROUP_DEVELOPER = $"{GROUP_ADMIN}, {COLOR_BLUE}";

        public readonly List<string> Perms;

        public Permission() => Perms = new List<string>();

        /// <summary>
        /// Check if this Contains Permission
        /// </summary>
        /// <param name="perm">Permisison</param>
        public bool this[string perm] => Perms.Any(p => perm == p);

        /// <summary>
        /// Generate Permissions Class from String based on "Perm1, Perm2, Perm3"
        /// </summary>
        /// <param name="perm">Permisison</param>
        public static Permission From(string perm) => new Permission() + perm.Split(", ");

        /// <summary>
        /// Add string as Permission to This
        /// </summary>
        /// <param name="perm">Permisison</param>
        public void Add(string perm)
        {
            Perms.Add(perm);
        }

        /// <summary>
        /// Add string[] as Permission to This
        /// </summary>
        /// <param name="perm">Permisison</param>
        public void Add(IEnumerable<string> perm)
        {
            Perms.AddRange(perm);
        }

        /// <summary>
        /// Add string as Permission to This
        /// </summary>
        /// <param name="instance">This</param>
        /// <param name="perm">Permission</param>
        /// <returns>This</returns>
        public static Permission operator +(Permission instance, string perm)
        {
            instance.Add(perm);
            return instance;
        }

        /// <summary>
        /// Add Permissions from Other Instance to This
        /// </summary>
        /// <param name="instance">This</param>
        /// <param name="otherInstance">Other</param>
        /// <returns>This</returns>
        public static Permission operator +(Permission instance, Permission otherInstance)
        {
            instance.Add(otherInstance.Perms);
            return instance;
        }

        /// <summary>
        /// Add string[] as Permission to This
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="perms"></param>
        /// <returns>This</returns>
        public static Permission operator +(Permission instance, IEnumerable<string> perms)
        {
            instance.Add(perms);
            return instance;
        }

        /// <summary>
        /// Remove Permission as String from This
        /// </summary>
        /// <param name="perm">Permission</param>
        public void Remove(string perm)
        {
            Perms.Remove(perm);
        }

        /// <summary>
        /// Remove Permissions as String[] from This
        /// </summary>
        /// <param name="perm">Permission</param>
        public void Remove(IEnumerable<string> perm)
        {
            Perms.RemoveAll(x => perm.Any(p => x == p));
        }

        /// <summary>
        /// Remove Permission as String from This
        /// </summary>
        /// <param name="instance">This</param>
        /// <param name="perm">Permission</param>
        public static Permission operator -(Permission instance, string perm)
        {
            instance.Remove(perm);
            return instance;
        }

        /// <summary>
        /// Check if This has Permission based on String
        /// </summary>
        /// <param name="perm">Permission</param>
        /// <returns>Has Permission</returns>
        public bool HasPermission(string perm) => Perms.Any(p => perm == p);

        /// <summary>
        /// Check if This has Permission based on String
        /// </summary>
        /// <param name="instance">This</param>
        /// <param name="perm">Permission</param>
        /// <returns>Has Permission</returns>
        public static bool operator ==(Permission instance, string perm) => instance?.HasPermission(perm) ?? true;

        /// <summary>
        /// Check if This doesn't has Permission based on String
        /// </summary>
        /// <param name="instance">This</param>
        /// <param name="perm">Permission</param>
        /// <returns>Has No Permission</returns>
        public static bool operator !=(Permission instance, string perm) => !(instance?.HasPermission(perm) ?? true);

        /// <summary>
        /// Check if This has Permission based on Permission
        /// </summary>
        /// <param name="instance">This</param>
        /// <param name="other">Permission</param>
        /// <returns>Has Permission</returns>
        public static bool operator ==(Permission instance, Permission other)
            => instance?.HasPermission(other?.ToString() ?? "") ?? true;

        /// <summary>
        /// Check if This doesn't has Permission based on Permission
        /// </summary>
        /// <param name="instance">This</param>
        /// <param name="other">Permission</param>
        /// <returns>Has No Permission</returns>
        public static bool operator !=(Permission instance, Permission other)
            => !(instance?.HasPermission(other?.ToString() ?? "") ?? true);

        private bool Equals(Permission other) => Equals(ToString(), other.ToString());

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            return obj.GetType() == GetType() && Equals((Permission) obj);
        }

        public override int GetHashCode() => Perms != null ? Perms.GetHashCode() : 0;

        public override string ToString() => string.Join(", ", Perms.ToArray());
    }
}
