namespace Praxent.Data.SQL
{
    public interface ISQLDbContextFactory<T> where T : class, IDataEntity, new()
    {
        SQLDbContext<T> GetContext();
    }
}