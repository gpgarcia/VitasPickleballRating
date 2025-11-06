using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace TestPickleBallApi
{
    [TestClass]
    public class ProgramTests
    {
        [TestMethod]
        public void TestConfigureServices()
        {
            // Arrange
            var builder = WebApplication.CreateBuilder();
            // Act
            PickleBallAPI.Program.ConfigureServices(builder);
            var app = builder.Build();

            // Assert
            Assert.IsNotNull(app);
            var mapper = app.Services.GetService<IMapper>();
            Assert.IsNotNull(mapper);

            mapper.ConfigurationProvider.AssertConfigurationIsValid();

        }

    }
}
