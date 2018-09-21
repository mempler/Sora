using Sora.Enums;
using Sora.Helpers;

namespace Sora.Packets
{
    internal class LoginResponse : IPacketSerializer
    {
        protected int Response;
        public LoginResponse(LoginResponses response) => Response = (int)response;
        public LoginResponse(int response) => Response = response;

        public PacketId Id => PacketId.ServerAnnounce;

        public void Read_from_stream(MStreamReader sr)
        {
            Response = sr.ReadInt32();
        }

        public void Write_to_stream(MStreamWriter sw)
        {
            sw.Write(Response);
        }
    }
}
