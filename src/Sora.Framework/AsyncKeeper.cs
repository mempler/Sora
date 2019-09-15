using System.Collections.Generic;
using System.Threading;

namespace Sora.Framework
{
    public interface IAsyncKeeper<I, T>
    {
        void Push(I key, T val);
        void Pop(I key);
        bool TryGet(I key, out T val);
    }
    
    public class AsyncKeeper<I, T> : IAsyncKeeper<I, T>
    {
        protected readonly Dictionary<I, T> Values =
            new Dictionary<I, T>();

        protected readonly ReaderWriterLock RWL = new ReaderWriterLock();

        public void Push(I key, T val)
        {
            try
            {
                RWL.AcquireReaderLock(1000); // this is already too long but who cares ?
                
                Values.Add(key, val);
            }
            finally
            {
                RWL.ReleaseReaderLock();
            }
        }

        public void Pop(I key)
        {
            try
            {
                RWL.AcquireReaderLock(1000); // this is already too long but who cares ?

                Values.Remove(key);
            }
            finally
            {
                RWL.ReleaseReaderLock();
            }
        }


        public bool TryGet(I key, out T val)
        {
            try
            {
                RWL.AcquireReaderLock(500);

                return Values.TryGetValue(key, out val);
            }
            finally
            {
                RWL.ReleaseReaderLock();
            }
        }
    }
}