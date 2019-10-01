using System.Collections.Generic;
using System.Linq;
using Sora.Framework.Objects;
using Sora.Framework.Utilities;

namespace Sora.Framework
{
    public interface IPresenceKeeper : IPacketPusher
    {
        void Join(Presence pr);
        void Leave(Presence pr);
        bool TryGet(string token, out Presence pr);
        bool TryGet(int userId, out Presence pr);
    }
    
    public class PresenceKeeper : AsyncKeeper<Token, Presence>, IPresenceKeeper
    {
        public PresenceKeeper()
        {
            Values = new Dictionary<Token, Presence>(new TokenEqualityComparer());
        }
        
        public void Push(IPacket packet, Presence skip = null)
        {
            try
            {
                RWL.AcquireReaderLock(1000); // this is already too long but who cares ?

                foreach (var pr in Values.Where(pr => pr.Value != skip))
                {
                    pr.Value.Push(packet);
                }
            }
            finally
            {
                RWL.ReleaseReaderLock();
            }
        }

        public virtual void Join(Presence sender)
        {
            try
            {
                RWL.AcquireWriterLock(50);
                Values[sender.Token] = sender;
            }
            finally
            {
                RWL.ReleaseWriterLock();
            }
        }

        public virtual void Leave(Presence sender)
        {
            try
            {
                RWL.AcquireWriterLock(50);
                Values.Remove(sender.Token);
            }
            finally
            {
                RWL.ReleaseWriterLock();
            }
        }

        public bool TryGet(int userId, out Presence pr)
        {
            try
            {
                RWL.AcquireReaderLock(500);

                pr = Values.FirstOrDefault(_pr => _pr.Value.User.Id == userId).Value;
                return pr != null;
            }
            finally
            {
                RWL.ReleaseReaderLock();
            }
        }

        public bool TryGet(string userName, out Presence pr)
        {
            try
            {
                RWL.AcquireReaderLock(500);

                pr = Values.FirstOrDefault(_pr => _pr.Value.User.UserName == userName).Value;
                return pr != null;
            }
            finally
            {
                RWL.ReleaseReaderLock();
            }
        }
    }
}