using Sora.Framework.Packets.Server;

namespace Sora.Framework.Objects
{
    public class Spectator : PresenceKeeper
    {
        public void MissingMap(Presence sender)
        {
            Push(new SpectatorCantSpectate(sender.User.Id));
        }
        
        public override void Join(Presence sender)
        {
            base.Join(sender);
            
            Push(new SpectatorJoined(sender.User.Id));
            Push(new FellowSpectatorJoined(sender.User.Id));
        }

        public override void Leave(Presence sender)
        {
            base.Leave(sender);
            
            Push(new SpectatorLeft(sender.User.Id));
            Push(new FellowSpectatorLeft(sender.User.Id));
        }
    }
}