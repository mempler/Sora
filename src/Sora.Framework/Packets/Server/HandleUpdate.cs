using Sora.Framework.Enums;
using Sora.Framework.Objects;
using Sora.Framework.Utilities;

namespace Sora.Framework.Packets.Server
{
    public class HandleUpdate : IPacket
    {
        public Presence Presence;

        public HandleUpdate(Presence presence) => Presence = presence;

        public PacketId Id => PacketId.ServerHandleOsuUpdate;

        public void ReadFromStream(MStreamReader sr)
        {
        }

        public void WriteToStream(MStreamWriter sw)
        {
            sw.Write(Presence.User.Id);
            sw.Write((byte) Presence.Status.Status);
            sw.Write(Presence.Status.StatusText, false);
            sw.Write(Presence.Status.BeatmapChecksum, false);
            sw.Write((uint) Presence.Status.CurrentMods);
            sw.Write((byte) Presence.Status.Playmode);
            sw.Write(Presence.Status.BeatmapId);

            sw.Write(Presence.Stats.RankedScore);
            sw.Write(Presence.Stats.Accuracy);
            sw.Write(Presence.Stats.PlayCount);
            sw.Write(Presence.Stats.TotalScore);
            sw.Write(Presence.Stats.Position);
            sw.Write(Presence.Stats.PerformancePoints);
        }
    }
}
