using Microsoft.EntityFrameworkCore;

namespace Praxent.Data.SQL
{
    public abstract class SQLDbContext<T> : DbContext where T : class, IDataEntity, new()
    {
        private readonly string _connectionString;

        public SQLDbContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        public abstract DbSet<T> GetEntitySet();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }
    }
}