using System;

using Kaoiji.enums;

namespace Kaoiji.packets
{
    public class LoginResponse : Packet
    {
        public LoginResponse(LoginResponses responses)
        {
            PacketID = PacketIDs.Server_LoginResponse;
            PacketData = BitConverter.GetBytes((int)responses);
        }
        public LoginResponse(int UserID)
        {
            PacketID = PacketIDs.Server_LoginResponse;
            PacketData = BitConverter.GetBytes((int)UserID);
        }
    }
}
