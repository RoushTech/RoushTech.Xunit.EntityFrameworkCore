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
            var directory = FindAppsettingsDirectory();
            Console.WriteLine($"Directory resolved to: {Directory.GetCurrentDirectory()}");
            Console.WriteLine($"Looking for appsettings at: {Directory.GetCurrentDirectory()}{directory}appsettings.json");
            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(Path.GetFullPath($"{directory}appsettings.json"), true)
                .AddJsonFile(Path.GetFullPath($"{directory}appsettings.local.json"), true)
                .AddEnvironmentVariables()
                .Build();
            ServiceCollection = new ServiceCollection();
            ManagedContexts = new List<Type>();
            foreach (var configuration in Configuration.GetSection("ConnectionStrings").GetChildren())
            {
                configuration.Value = configuration.Value.Replace("{ID}", InstanceId.ToString());
            }
        }

        public void EnableDatabaseDestruction()
        {
            AppDomain.CurrentDomain.ProcessExit += (s, e) => Dispose();
        }

        private string FindAppsettingsDirectory()
        {
            var directory = string.Empty;
            var depth = 0;
            while (!Check(directory) || depth > 6)
            {
                directory = "../" + directory;
                depth++;
            }

            return directory;
        }

        private bool Check(string directory)
        {
            var file = $"{directory}/appsettings.json";
            Console.WriteLine(file);
            return File.Exists(file);
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
                using (var dataContext = (DbContext)scope.ServiceProvider.GetService(dbContextType))
                {
                    action(dataContext.Database);
                }
            }
        }
    }
}