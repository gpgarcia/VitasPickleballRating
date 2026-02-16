using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace TestPickleBallApi;

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

    public void PickleballProfile_ShouldBeValid()
    {
        // Arrange
        var lf = LoggerFactory.Create(builder => builder.AddConsole());
        var config = new MapperConfiguration(cfg => cfg.AddProfile<PickleBallAPI.PickleBallProfile>(), lf);
        // Act & Assert
        config.AssertConfigurationIsValid();

    }
}
