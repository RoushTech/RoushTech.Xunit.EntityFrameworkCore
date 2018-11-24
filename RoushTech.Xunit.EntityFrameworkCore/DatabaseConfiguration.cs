namespace RoushTech.Xunit.EntityFrameworkCore
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Infrastructure;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public class DatabaseConfiguration
    {
        public IConfiguration Configuration { get; }

        public static DatabaseConfiguration Instance { get; } = new DatabaseConfiguration();

        public Guid InstanceId { get; } = Guid.NewGuid();

        public IServiceCollection ServiceCollection { get; }

        public IServiceProvider Services { get; protected set; }

        protected IList<Type> ManagedContexts { get; }

        protected DatabaseConfiguration()
        {
            Configuration = new ConfigurationBuilder()
                .AddJsonFile(Path.GetFullPath(@"..\..\..\appsettings.json"), true)
                .AddJsonFile(Path.GetFullPath(@"..\..\..\appsettings.local.json"), true)
                .AddEnvironmentVariables()
                .Build();
            ServiceCollection = new ServiceCollection();
            ManagedContexts = new List<Type>();
            foreach (var configuration in Configuration.GetSection("ConnectionStrings").GetChildren())
            {
                configuration.Value = configuration.Value.Replace("{ID}", InstanceId.ToString());
            }

            AppDomain.CurrentDomain.ProcessExit += (s, e) => Dispose();
        }

        public void AddContext<TDbContext>(Action<DbContextOptionsBuilder> options) where TDbContext : DbContext
        {
            ManagedContexts.Add(typeof(TDbContext));
            ServiceCollection.AddDbContext<TDbContext>(options);
        }

        public void CreateDatabases()
        {
            DoToAllDatabases(d => d.EnsureCreated());
        }

        public void CreateServiceProvider()
        {
            Services = ServiceCollection.BuildServiceProvider();
        }

        public void Dispose()
        {
            DoToAllDatabases(d => d.EnsureDeleted());
        }

        public void DoToAllDatabases(Action<DatabaseFacade> action)
        {
            foreach (var dbContextType in ManagedContexts)
            {
                using (var scope = Services.CreateScope())
                using (var dataContext = (DbContext) scope.ServiceProvider.GetService(dbContextType))
                {
                    action(dataContext.Database);
                }
            }
        }
    }
}