using Shared.Enums;
using Shared.Helpers;
using Shared.Interfaces;
using Sora.Enums;

namespace Sora.Packets.Server
{
    public class LoginPermission : IPacket
    {
        public LoginPermissions Permission;

        public LoginPermission(LoginPermissions perm) => Permission = perm;

        public PacketId Id => PacketId.ServerLoginPermissions;
        
        public void ReadFromStream(MStreamReader sr) => throw new System.NotImplementedException();

        public void WriteToStream(MStreamWriter sw) => sw.Write((int) Permission);
    }
}