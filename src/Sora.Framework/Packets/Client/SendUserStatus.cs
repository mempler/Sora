using Sora.Framework.Enums;
using Sora.Framework.Objects;
using Sora.Framework.Utilities;

namespace Sora.Framework.Packets.Client
{
    public class SendUserStatus : IPacket
    {
        public UserStatus Status;
        public PacketId Id => PacketId.ClientSendUserStatus;

        public void ReadFromStream(MStreamReader sr)
        {
            Status = new UserStatus
            {
                Status = (Status) sr.ReadByte(),
                StatusText = sr.ReadString(),
                BeatmapChecksum = sr.ReadString(),
                CurrentMods = (Mod) sr.ReadUInt32(),
                Playmode = (PlayMode) sr.ReadByte(),
                BeatmapId = sr.ReadUInt32()
            };
        }

        public void WriteToStream(MStreamWriter sw)
        {
            sw.Write((byte) Status.Status);
            sw.Write(Status.StatusText);
            sw.Write(Status.BeatmapChecksum);
            sw.Write((uint) Status.CurrentMods);
            sw.Write((byte) Status.Playmode);
            sw.Write(Status.BeatmapId);
        }
    }
}
