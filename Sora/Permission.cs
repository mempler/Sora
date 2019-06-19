using System.Collections.Generic;
using System.Linq;

namespace Sora
{
    public class Permission
    {
        private readonly List<string> _perms;

        public Permission()
        {
            _perms = new List<string>();
        }

        public void Add(string perm) => _perms.Add(perm);
        public void Add(IEnumerable<string> perm) => _perms.AddRange(perm);
        public static Permission operator +(Permission instance, string perm)
        {
            instance.Add(perm);
            return instance;
        }

        public void Remove(string perm) => _perms.Remove(perm);
        public void Remove(IEnumerable<string> perm) => _perms.RemoveAll(x => perm.Any(p => x == p));
        public static Permission operator -(Permission instance, string perm)
        {
            instance.Remove(perm);
            return instance;
        }

        public bool HasPermission(string perm) => _perms.Any(p => perm == p);
        public bool this[string perm] => _perms.Any(p => perm == p);
        
        public static bool operator ==(Permission instance, string perm) => instance?.HasPermission(perm) ?? true;
        public static bool operator !=(Permission instance, string perm) => !(instance?.HasPermission(perm) ?? true);

        private bool Equals(Permission other) => Equals(_perms, other._perms);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((Permission) obj);
        }

        public override int GetHashCode() => _perms != null ? _perms.GetHashCode() : 0;
    }
}