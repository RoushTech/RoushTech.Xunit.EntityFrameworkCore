namespace RoushTech.Xunit.EntityFrameworkCore.Tests.Setup
{
    using System;
    using Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;

    public class DatabaseConfig
    {
        public static DatabaseConfig Instance { get; } = new DatabaseConfig();

        public IServiceProvider Services => DatabaseConfiguration.Instance.Services;

        private DatabaseConfig()
        {
            DatabaseConfiguration.Instance.AddContext<DatabaseContext>(options => options.UseSqlServer(
                DatabaseConfiguration.Instance.Configuration.GetConnectionString("Default")));
            DatabaseConfiguration.Instance.CreateServiceProvider();
            DatabaseConfiguration.Instance.CreateDatabases();
        }
    }
}