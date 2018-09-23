using System;
using Shared.Enums;
using Shared.Helpers;
using Shared.Interfaces;
using Sora.Enums;

namespace Sora.Packets.Client
{
    public class SendUserStatus : IPacketSerializer
    {
        public PacketId Id => PacketId.ClientSendUserStatus;

        public UserStatus Status;

        public void ReadFromStream(MStreamReader sr) =>
            Status = new UserStatus
            {
                Status = (Status) sr.ReadByte(),
                StatusText = sr.ReadString(),
                BeatmapChecksum = sr.ReadString(),
                CurrentMods = sr.ReadUInt32(),
                Playmode = (PlayModes) sr.ReadByte(),
                BeatmapId = sr.ReadUInt32()
            };

        public void WriteToStream(MStreamWriter sw) =>
            throw new NotImplementedException();
    }

    public struct UserStatus
    {
        public Status Status;
        public string StatusText;
        public string BeatmapChecksum;
        public uint CurrentMods;
        public PlayModes Playmode;
        public uint BeatmapId;
    }
}
