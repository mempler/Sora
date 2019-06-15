using System;
using System.Collections.Generic;

namespace Sora.Database
{
    // Modified version of https://github.com/ppy/osu/blob/master/osu.Game/Database/DatabaseWriteUsage.cs under MIT License!
    public class DatabaseWriteUsage : IDisposable
    {
        public readonly SoraDbContext Context;
        private readonly Action<DatabaseWriteUsage> usageCompleted;

        public DatabaseWriteUsage(SoraDbContext context, Action<DatabaseWriteUsage> onCompleted)
        {
            Context        = context;
            usageCompleted = onCompleted;
        }

        public bool PerformedWrite { get; private set; }

        private bool isDisposed;
        public List<Exception> Errors = new List<Exception>();

        /// <summary>
        /// Whether this write usage will commit a transaction on completion.
        /// If false, there is a parent usage responsible for transaction commit.
        /// </summary>
        public bool IsTransactionLeader = false;

        protected void Dispose(bool disposing)
        {
            if (isDisposed) return;

            isDisposed = true;

            try
            {
                PerformedWrite |= Context.SaveChanges() > 0;
            }
            catch (Exception e)
            {
                Errors.Add(e);
                throw;
            }
            finally
            {
                usageCompleted?.Invoke(this);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~DatabaseWriteUsage()
        {
            Dispose(false);
        }
    }
}