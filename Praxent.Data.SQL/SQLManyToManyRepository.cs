using System.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Praxent.Data.SQL.Extensions;

namespace Praxent.Data.SQL
{
    public class SQLManyToManyRepository<T> : ICompositeKeyRepository<T> where T : class, new()
    {
        private readonly ISQLBasicDbContextFactory<T> _dbContextFactory;

        public SQLManyToManyRepository(ISQLBasicDbContextFactory<T> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public IQueryable<T> GetAll()
        {
            /*
             * Why no using?
             * We don't have to - https://blog.jongallant.com/2012/10/do-i-have-to-call-dispose-on-dbcontext/
             * We don't want to - We're returning an IQueryable, which will be opened and closed at the App Surface, likely an API
             */
            var context = _dbContextFactory.GetContext();
            var entitySet = context.GetEntitySet();
            var query = entitySet.IncludePropertyPaths(context.GetIncludePaths(typeof(T)));

            return query;
        }

        public T GetByKey(params object[] keyValues)
        {
            using (var context = _dbContextFactory.GetContext())
            {
                return context.Set<T>().Find(keyValues);
            }
        }

        public T Add(T entity)
        {
            using (var context = _dbContextFactory.GetContext())
            {
                var entitySet = context.GetEntitySet();
                var query = entitySet.IncludePropertyPaths(context.GetIncludePaths(typeof(T)));

                entitySet.Add(entity);
                context.SaveChanges();
                return entity;
            }
        }

        public T Update(T entity, params object[] keyValues)
        {
            using (var context = _dbContextFactory.GetContext())
            {
                var entitySet = context.GetEntitySet();
                var query = entitySet.IncludePropertyPaths(context.GetIncludePaths(typeof(T)));
                var foundEntity = entitySet.Find(keyValues);
                if (foundEntity == null)
                {
                    //TODO: Upsert or Error?
                    throw new DataException("Cannot Update Entity that doesn't exist!");
                }

                context.Entry(foundEntity).State = EntityState.Detached;
                context.Entry(entity).State = EntityState.Modified;
                entitySet.Update(entity);
                context.SaveChanges();
                var updatedEntity = context.Set<T>().Find(keyValues);

                return updatedEntity;
            }
        }

        public T Delete(params object[] keyValues)
        {
            using (var context = _dbContextFactory.GetContext())
            {
                var entitySet = context.GetEntitySet();
                var foundEntity = context.Set<T>().Find(keyValues);
                if (foundEntity == null)
                {
                    //TODO: Silently Complete or Error?
                    throw new DataException("Cannot Delete Entity that doesn't exist!");
                }

                entitySet.Remove(foundEntity);
                context.SaveChanges();

                return foundEntity;
            }
        }
    }
}