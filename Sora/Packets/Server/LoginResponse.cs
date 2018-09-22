using Shared.Enums;
using Shared.Helpers;
using Shared.Interfaces;
using Sora.Enums;

namespace Sora.Packets.Server
{
    internal class LoginResponse : IPacketSerializer
    {
        protected LoginResponses Response;
        public LoginResponse(LoginResponses response) => Response = response;

        public PacketId Id => PacketId.ServerLoginResponse;

        public void ReadFromStream(MStreamReader sr) => Response = (LoginResponses)sr.ReadInt32();

        public void WriteToStream(MStreamWriter sw) => sw.Write((int)Response);
    }
}
