using Sora.Packets.Server;

namespace Sora.Objects
{
    public partial class Presence
    {
        public void Alert(string Message)
        {
            Write(new Announce(Message));
        }

        public void SendMessage(Channel channel, string Message)
        {
            channel.WriteMessage(this, Message, Get<bool>("BOT"));
        }
    }
}
