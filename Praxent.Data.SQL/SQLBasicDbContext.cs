using Microsoft.EntityFrameworkCore;

namespace Praxent.Data.SQL
{
    public abstract class SQLBasicDbContext<T> : DbContext where T : class, new()
    {
        private readonly string _connectionString;

        public SQLBasicDbContext(string connectionString)
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