namespace RoushTech.Xunit.EntityFrameworkCore
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public abstract class DbContextTest<TDbContext> : IDisposable where TDbContext : DbContext
    {
        protected IConfiguration Configuration => DatabaseConfiguration.Instance.Configuration;

        protected TDbContext DbContext { get; set; }

        protected IServiceProvider ServiceProvider => ServiceScope.ServiceProvider;

        protected IServiceScope ServiceScope { get; private set; }
        
        protected void BeginTransaction()
        {   
            DbContext.Database.BeginTransaction();
        }

        protected void AddContext<T>(Action<DbContextOptionsBuilder> options) where T : DbContext
        {
            DatabaseConfiguration.Instance.AddContext<T>(options);
        }

        protected void SeedDatabase()
        {
            lock (DatabaseConfiguration.Instance.ConcurrencyLock)
            {
                if (DatabaseConfiguration.Instance.Seeded)
                {
                    return;
                }
                
                Seed().Wait();
                DatabaseConfiguration.Instance.Seeded = true;
            }
        }

        protected abstract Task Seed();

        protected void CreateServiceProvider(Action<IServiceCollection> services)
        {
            lock (DatabaseConfiguration.Instance.ConcurrencyLock)
            {
                if (DatabaseConfiguration.Instance.Services == null)
                {
                    services(DatabaseConfiguration.Instance.ServiceCollection);
                    DatabaseConfiguration.Instance.CreateServiceProvider();
                }
            }

            ServiceScope = DatabaseConfiguration.Instance.Services.CreateScope();
            DbContext = ServiceScope.ServiceProvider.GetService<TDbContext>();
        }

        public void Dispose()
        {
            DbContext?.Database?.RollbackTransaction();
            ServiceScope?.Dispose();
        }
    }
}