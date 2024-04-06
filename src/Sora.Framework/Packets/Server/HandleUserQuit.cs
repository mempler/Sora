using Sora.Framework.Enums;
using Sora.Framework.Utilities;

namespace Sora.Framework.Packets.Server
{
    public class HandleUserQuit : IPacket
    {
        public UserQuitStruct UserQuit;

        public HandleUserQuit(UserQuitStruct userQuit) => UserQuit = userQuit;

        public PacketId Id => PacketId.ServerHandleUserQuit;

        public void ReadFromStream(MStreamReader sr)
        {
            UserQuit = new UserQuitStruct {UserId = sr.ReadInt32(), ErrorState = (ErrorStates) sr.ReadInt32()};
        }

        public void WriteToStream(MStreamWriter sw)
        {
            sw.Write(UserQuit.UserId);
            sw.Write((int) UserQuit.ErrorState);
        }
    }

    public struct UserQuitStruct
    {
        public int UserId;
        public ErrorStates ErrorState;
    }
}
