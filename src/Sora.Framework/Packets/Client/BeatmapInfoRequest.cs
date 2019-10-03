using System.Collections.Generic;
using Sora.Framework.Enums;
using Sora.Framework.Utilities;

namespace Sora.Framework.Packets.Client
{
    public class BeatmapInfoRequest : IPacket
    {
        public PacketId Id => PacketId.ClientBeatmapInfoRequest;

        public readonly List<string> FileNames = new List<string>();
        public readonly List<int> Ids = new List<int>();
        
        public void ReadFromStream(MStreamReader sr)
        {
            var c = sr.ReadInt32();
            for (var i = 0; i < c; i++)
                FileNames.Add(sr.ReadString());

            c = sr.ReadInt32();
            for (var i = 0; i < c; i++)
                Ids.Add(sr.ReadInt32());
        }

        public void WriteToStream(MStreamWriter sw)
        {
            throw new System.NotImplementedException();
        }
    }
}