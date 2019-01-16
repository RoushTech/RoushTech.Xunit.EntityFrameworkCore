namespace RoushTech.Xunit.EntityFrameworkCore.Tests
{
    using global::Xunit;

    public class ConfigurationTest
    {
        [Fact]
        public void LoadConfig()
        {
            Assert.Equal("abc123", DatabaseConfiguration.Instance.Configuration["test"]);
        }
    }
}