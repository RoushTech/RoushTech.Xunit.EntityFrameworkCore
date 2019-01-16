namespace RoushTech.Xunit.EntityFrameworkCore.Tests.Entities
{
    using Microsoft.EntityFrameworkCore;
    using Models;

    public class DatabaseContext : DbContext
    {
        public DbSet<TestModel> TestModels { get; set; }
        
        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
        {
        }
    }
}