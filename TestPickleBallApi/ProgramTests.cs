using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using PickleBallAPI.Controllers;
using PickleBallAPI;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

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
