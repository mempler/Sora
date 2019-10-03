using System.Collections.Generic;
using Sora.Framework.Enums;
using Sora.Framework.Utilities;

namespace Sora.Framework.Packets.Server
{
    public class BeatmapInfo
    {
        public int BeatmapId;
        public int SetId;
        public RankedStatus RankedStatus;
        public Rank OsuRank = Rank.N;
        public Rank TaikoRank = Rank.N;
        public Rank CatchRank = Rank.N;
        public Rank ManiaRank = Rank.N;
        public string FileMd5;
    }
    
    public class BanchoInfoReply : IPacket
    {
        public List<BeatmapInfo> Info;
        
        public PacketId Id { get; }
        
        public void ReadFromStream(MStreamReader sr)
        {
            throw new System.NotImplementedException();
        }

        public void WriteToStream(MStreamWriter sw)
        {
            sw.Write(Info.Count);
            
            ushort i = 0;
            foreach (var info in Info)
            {
                sw.Write(i);
                sw.Write(info.BeatmapId);
                sw.Write(info.SetId);
                sw.Write(0); // ???
                sw.Write((byte) info.OsuRank);
                sw.Write((byte) info.TaikoRank);
                sw.Write((byte) info.CatchRank);
                sw.Write((byte) info.ManiaRank);
                sw.Write(info.FileMd5);
                i++;
            }
        }
    }
}