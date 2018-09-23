using Shared.Enums;
using Shared.Handlers;
using Sora.Objects;
using Sora.Packets.Client;
using Sora.Packets.Server;

namespace Sora.Handler
{
    internal class UserStatusHandler
    {
        [Handler(HandlerTypes.ClientSendUserStatus)]
        private void HandleUserStatus(Presence pr, UserStatus status)
        {
            pr.Status = status;
            LPacketStreams.GetStream("main").Broadcast(new HandleUpdate(pr));
        }
    }
}
