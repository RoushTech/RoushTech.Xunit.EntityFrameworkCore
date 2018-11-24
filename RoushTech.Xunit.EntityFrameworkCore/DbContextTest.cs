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

        protected IServiceScope ServiceScope { get; set; }
        
        protected void Setup()
        {
            DbContext.Database.BeginTransaction();
        }
 
        public void Dispose()
        {
            DbContext?.Database?.RollbackTransaction();
            ServiceScope?.Dispose();
        }
    }
}