using System;
using Sora.Enums;
using Sora.Helpers;
using Sora.Interfaces;

namespace Sora.Packets.Server
{
    public class LoginPermission : IPacket
    {
        public LoginPermissions Permission;

        public LoginPermission(LoginPermissions perm) => Permission = perm;

        public PacketId Id => PacketId.ServerLoginPermissions;

        public void ReadFromStream(MStreamReader sr)
        {
            throw new NotImplementedException();
        }

        public void WriteToStream(MStreamWriter sw)
        {
            sw.Write((int) Permission);
        }
    }
}
