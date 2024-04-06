using Sora.Framework.Enums;
using Sora.Framework.Utilities;

namespace Sora.Framework.Packets.Server
{
    public class LoginResponse : IPacket
    {
        protected LoginResponses Response;

        public LoginResponse(LoginResponses response) => Response = response;

        public PacketId Id => PacketId.ServerLoginResponse;

        public void ReadFromStream(MStreamReader sr)
        {
            Response = (LoginResponses) sr.ReadInt32();
        }

        public void WriteToStream(MStreamWriter sw)
        {
            sw.Write((int) Response);
        }
    }
}
