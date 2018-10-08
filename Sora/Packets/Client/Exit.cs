using Shared.Enums;
using Shared.Helpers;
using Shared.Interfaces;
using Sora.Enums;

namespace Sora.Packets.Client
{
    public class Exit : IPacket
    {
        public PacketId Id => PacketId.ClientExit;
        public ErrorStates ErrorState;
        

        public void ReadFromStream(MStreamReader sr) => this.ErrorState = (ErrorStates) sr.ReadInt32();

        public void WriteToStream(MStreamWriter sw) => sw.Write((int) this.ErrorState);
    }
}
