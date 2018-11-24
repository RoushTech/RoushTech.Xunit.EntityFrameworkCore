namespace RoushTech.Xunit.EntityFrameworkCore.Tests
{
    using System.Threading.Tasks;
    using Entities.Models;
    using global::Xunit;
    using Microsoft.EntityFrameworkCore;
    using Setup;

    public class DatabaseTests : DbContextTest
    {
        [Fact]
        public async Task Test()
        {
            var testModel = new TestModel();
            DbContext.TestModels.Add(testModel);
            await DbContext.SaveChangesAsync();
            Assert.NotEqual(0, testModel.Id);
            var databaseModel = await DbContext.TestModels.AsNoTracking().FirstOrDefaultAsync();
            Assert.Equal(testModel.Id, databaseModel.Id);
        }
    }
}