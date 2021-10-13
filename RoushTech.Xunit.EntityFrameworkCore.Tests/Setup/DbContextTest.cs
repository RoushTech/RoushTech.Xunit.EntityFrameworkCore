namespace RoushTech.Xunit.EntityFrameworkCore.Tests.Setup
{
    using Entities;
    using Microsoft.Extensions.DependencyInjection;

    public class DbContextTest : DbContextTest<DatabaseContext>
    {
        public DbContextTest()
        {
            ServiceScope = DatabaseConfig.Instance.Services.CreateScope();
            DbContext = ServiceProvider.GetService<DatabaseContext>();
        }
    }
}