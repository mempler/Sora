using System;

namespace Sora.Framework.Objects.Multiplayer
{
    public class Lobby : PresenceKeeper
    {
        public static Lobby Self { get;  } = new Lobby();

        public void Push(Match match)
        {
            throw new NotImplementedException();
        }

        public void Pop(Match match)
        {
            throw new NotImplementedException();
        }
    }
}