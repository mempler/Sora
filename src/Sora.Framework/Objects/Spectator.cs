using Sora.Framework.Packets.Server;

namespace Sora.Framework.Objects
{
    public class Spectator : PresenceKeeper
    {
        public Presence Host { get; }
        public int SpectatorCount => Values.Count - 1;
        
        public Channel Channel { get; } = new Channel{Name = "#spectator"};

        public Spectator(Presence host)
        {
            Host = host;
        }
        
        public void MissingMap(Presence sender)
        {
            Push(new SpectatorCantSpectate(sender.User.Id));
        }
        
        public override void Join(Presence sender)
        {
            base.Join(sender);

            if (sender == Host)
                return;
            
            Push(new SpectatorJoined(sender.User.Id));
            Push(new FellowSpectatorJoined(sender.User.Id));
        }

        public override void Leave(Presence sender)
        {
            base.Leave(sender);

            if (sender == Host)
                return;
            
            Push(new SpectatorLeft(sender.User.Id));
            Push(new FellowSpectatorLeft(sender.User.Id));
        }
    }
}