using Sora.Enums;
using IPacket = Sora.Interfaces.IPacket;
using MStreamReader = Sora.Helpers.MStreamReader;
using MStreamWriter = Sora.Helpers.MStreamWriter;
using PacketId = Sora.Enums.PacketId;

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