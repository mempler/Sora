using System;
using System.Threading;
using Ripple.MergeTool.Database;
using Ripple.MergeTool.Database.Model;
using Ripple.MergeTool.Tools;
using Sora.Database;
using Sora.Database.Models;
using Sora.Helpers;

namespace Ripple.MergeTool
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            Logger.Info("Ripple Merger v0.0.1a");
            
            SoraDbContextFactory factory = new SoraDbContextFactory();
            RippleDbContext rippleCtx = new RippleDbContext();
            
            using DatabaseWriteUsage db = factory.GetForWrite();

            foreach (RippleUsers user in rippleCtx.Users)
            {
                if (user.id == 999) // We don't need fokabot! gtfu
                    continue;

                if (user.email == null || user.password_md5 == null || user.username == null)
                    continue;
                
                Logger.Info($"User: {user.username} ( {user.id} )");

                db.Context.Users.Add(new Users
                {
                    Username   = user.username,
                    Password   = user.password_md5,
                    Privileges = PrivilegeMerger.Merge(user.privileges),
                    Email      = user.email
                });
            }
        }
    }
}