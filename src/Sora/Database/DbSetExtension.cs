// https://stackoverflow.com/questions/36208580/what-happened-to-addorupdate-in-ef-7-core

using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Sora.Database.Models;

namespace Sora.Database
{
    public static class DbSetExtension
    {
        public static void AddOrUpdate(this DbSet<DbBeatmap> dbSet, DbBeatmap data)
        {
            var context = dbSet.GetContext();

            var entities = dbSet.AsNoTracking().Where(set => set.Id == data.Id);

            var dbVal = entities.FirstOrDefault();
            if (dbVal != null)
            {
                context.Entry(dbVal).CurrentValues.SetValues(data);
                context.Entry(dbVal).State = EntityState.Modified;
                return;
            }

            dbSet.Add(data);
        }
    }


    public static class HackyDbSetGetContextTrick
    {
        public static DbContext GetContext<TEntity>(this DbSet<TEntity> dbSet)
            where TEntity : class
        {
            return (DbContext) dbSet
                               .GetType().GetTypeInfo()
                               .GetField("_context", BindingFlags.NonPublic | BindingFlags.Instance)
                               ?.GetValue(dbSet);
        }
    }
}