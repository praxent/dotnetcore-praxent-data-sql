namespace Praxent.Data.SQL
{
    public interface ISQLBasicDbContextFactory<T> where T : class, new()
    {
        SQLBasicDbContext<T> GetContext();
    }
}