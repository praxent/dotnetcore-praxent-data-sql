using Autofac;

namespace Praxent.Data.SQL
{
    public static class Bootstrapper
    {
        public static ContainerBuilder AddSQLDataComponents(this ContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof(SQLRepository<>)).As(typeof(IRepository<>));
            builder.RegisterGeneric(typeof(SQLManyToManyRepository<>)).As(typeof(ICompositeKeyRepository<>));

            return builder;
        }
    }
}