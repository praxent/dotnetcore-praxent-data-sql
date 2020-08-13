using System.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Praxent.Data.SQL.Extensions;

namespace Praxent.Data.SQL
{
    public class SQLRepository<T> : IRepository<T> where T : class, IDataEntity, new()
    {
        private readonly ISQLDbContextFactory<T> _dbContextFactory;

        public SQLRepository(ISQLDbContextFactory<T> dbContextFactory)
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

        public T GetById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new DataException("An ID must be provided to locate an Entity!");
            }
            using (var context = _dbContextFactory.GetContext())
            {
                var entitySet = context.GetEntitySet();
                var query = entitySet.IncludePropertyPaths(context.GetIncludePaths(typeof(T)));
                return query.FirstOrDefault(entity => entity.ID == id);
            }
        }

        public T Add(T entity)
        {
            //TODO: Upsert or Error?
            //if (!string.IsNullOrEmpty(entity.ID))
            //{
            //    throw new DataException("Entity cannot provide an ID before creation!");
            //}
            using (var context = _dbContextFactory.GetContext())
            {
                var entitySet = context.GetEntitySet();
                var query = entitySet.IncludePropertyPaths(context.GetIncludePaths(typeof(T)));
                //var foundEntity = context.EntitySet.FirstOrDefault(searchEntity => searchEntity.ID == entity.ID);
                //if (foundEntity != null)
                //{
                //    //TODO: Upsert or Error?
                //    throw new Exception("Cannot Add Entity with an ID that already exists!");
                //}

                entitySet.Add(entity);
                context.SaveChanges();
                var createdEntity = query.FirstOrDefault(searchEntity => searchEntity.ID == entity.ID);

                return createdEntity;
            }
        }

        public T Update(T entity)
        {
            if (string.IsNullOrEmpty(entity.ID))
            {
                throw new DataException("Entity must provide an ID for an Update!");
            }
            using (var context = _dbContextFactory.GetContext())
            {
                var entitySet = context.GetEntitySet();
                var query = entitySet.IncludePropertyPaths(context.GetIncludePaths(typeof(T)));
                var foundEntity = entitySet.FirstOrDefault(searchEntity => searchEntity.ID == entity.ID);
                if (foundEntity == null)
                {
                    //TODO: Upsert or Error?
                    throw new DataException(string.Format("Cannot Update Entity with an ID that doesn't exist! ID {0} does not exist!", entity.ID));
                }

                context.Entry(foundEntity).State = EntityState.Detached;
                context.Entry(entity).State = EntityState.Modified;
                entitySet.Update(entity);
                context.SaveChanges();
                var updatedEntity = query.FirstOrDefault(searchEntity => searchEntity.ID == entity.ID);

                return updatedEntity;
            }
        }

        public T Delete(T entity)
        {
            if (string.IsNullOrEmpty(entity.ID))
            {
                throw new DataException("Entity must provide an ID for a Delete!");
            }
            using (var context = _dbContextFactory.GetContext())
            {
                var entitySet = context.GetEntitySet();
                var foundEntity = entitySet.FirstOrDefault(searchEntity => searchEntity.ID == entity.ID);
                if (foundEntity == null)
                {
                    //TODO: Silently Complete or Error?
                    throw new DataException(string.Format("Cannot Delete Entity with an ID that doesn't exist! ID {0} does not exist!", entity.ID));
                }

                entitySet.Remove(foundEntity);
                context.SaveChanges();

                return foundEntity;
            }
        }

        public T Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new DataException("An ID must be provided to locate an Entity to Delete!");
            }
            using (var context = _dbContextFactory.GetContext())
            {
                var entitySet = context.GetEntitySet();
                var foundEntity = entitySet.FirstOrDefault(searchEntity => searchEntity.ID == id);
                if (foundEntity == null)
                {
                    //TODO: Silently Complete or Error?
                    throw new DataException(string.Format("Cannot Delete Entity with an ID that doesn't exist! ID {0} does not exist!", id));
                }

                entitySet.Remove(foundEntity);
                context.SaveChanges();

                return foundEntity;
            }
        }
    }
}