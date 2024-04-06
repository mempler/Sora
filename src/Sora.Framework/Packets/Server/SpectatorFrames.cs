using System;
using Sora.Framework.Enums;
using Sora.Framework.Packets.Client;
using Sora.Framework.Utilities;

namespace Sora.Framework.Packets.Server
{
    public class SpectatorFrames : IPacket
    {
        public readonly SpectatorFrame Frame;

        public SpectatorFrames(SpectatorFrame frames) => Frame = frames;

        public PacketId Id => PacketId.ServerSpectateFrames;

        public void ReadFromStream(MStreamReader sr)
        {
            throw new NotImplementedException();
        }

        public void WriteToStream(MStreamWriter sw)
        {
            sw.Write(Frame);
        }
    }
}
