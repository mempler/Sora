using Sora.Framework.Enums;
using Sora.Framework.Utilities;

namespace Sora.Framework.Packets.Client
{
    public class Exit : IPacket
    {
        public ErrorStates ErrorState;
        public PacketId Id => PacketId.ClientExit;

        public void ReadFromStream(MStreamReader sr)
        {
            ErrorState = (ErrorStates) sr.ReadInt32();
        }

        public void WriteToStream(MStreamWriter sw)
        {
            sw.Write((int) ErrorState);
        }
    }
}
