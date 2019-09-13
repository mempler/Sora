using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Sora.Framework.Allocation;
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
    
    public class PresenceKeeper : DynamicValues, IPresenceKeeper
    {
        protected readonly Dictionary<string, Presence> Presences =
            new Dictionary<string, Presence>();

        protected readonly ReaderWriterLock RWL = new ReaderWriterLock();
        
        public void Push(IPacket packet)
        {
            try
            {
                RWL.AcquireReaderLock(1000); // this is already too long but who cares ?

                foreach (var pr in Presences)
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
                Presences[sender.Token] = sender;
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
                Presences.Remove(sender.Token);
            }
            finally
            {
                RWL.ReleaseWriterLock();
            }
        }

        public bool TryGet(string token, out Presence pr)
        {
            try
            {
                RWL.AcquireReaderLock(500);

                return Presences.TryGetValue(token, out pr);
            }
            finally
            {
                RWL.ReleaseReaderLock();
            }
        }

        public bool TryGet(int userId, out Presence pr)
        {
            try
            {
                RWL.AcquireReaderLock(500);

                pr = Presences.FirstOrDefault(_pr => _pr.Value.User.Id == userId).Value;
                return pr != null;
            }
            finally
            {
                RWL.ReleaseReaderLock();
            }
        }
    }
}