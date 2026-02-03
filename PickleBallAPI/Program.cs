using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using PickleBallAPI.Models;
using System;
using System.IO;
using System.Reflection;

namespace PickleBallAPI;

/// <summary>
/// Entry point and DI/service configuration for the PickleBallAPI application.
/// </summary>
/// <remarks>
/// This class contains the minimal program bootstrap for an ASP.NET Core application:
/// - Builds a <see cref="WebApplication"/> using <see cref="WebApplication.CreateBuilder(string[])"/>.
/// - Registers application services and infrastructure in <see cref="ConfigureServices(WebApplicationBuilder)"/>.
/// - Configures middleware and runs the application.
/// 
/// The project targets .NET 8 and uses AutoMapper, Entity Framework Core, and Swagger for API documentation.
/// Conditional compilation (DEBUG) enables additional validation and logging behavior in development builds.
/// </remarks>
public static class Program
{
    /// <summary>
    /// Application entry point. Builds and runs the web host.
    /// </summary>
    /// <param name="args">Command-line arguments forwarded to <see cref="WebApplication.CreateBuilder(string[])"/>.</param>
    /// <returns>An integer exit code. The method returns 0 on normal termination (the host blocks until shutdown).</returns>
    /// <remarks>
    /// The method performs the following steps:
    /// 1. Create the <see cref="WebApplicationBuilder"/> and register services via <see cref="ConfigureServices(WebApplicationBuilder)"/>.
    /// 2. Build the <see cref="WebApplication"/>.
    /// 3. When in the Development environment, enable Swagger/OpenAPI UI.
    /// 4. In DEBUG builds, validate AutoMapper configuration at startup to fail fast on mapping issues.
    /// 5. Configure common middleware: HTTPS redirection, authorization, and controller routing.
    /// 6. Run the application which blocks until shutdown.
    /// 
    /// Note: The method returns 0 after calling <see cref="WebApplication.Run"/>; the host lifecycle is managed by ASP.NET Core.
    /// </remarks>
    public static int Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        ConfigureServices(builder);
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger(options =>
            {
                options.OpenApiVersion = OpenApiSpecVersion.OpenApi2_0;
            });
            app.UseSwaggerUI();
        }
#if DEBUG
        var mapper = app.Services.GetRequiredService<IMapper>();
        mapper.ConfigurationProvider.AssertConfigurationIsValid();
#endif
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
        return 0;
    }

    /// <summary>
    /// Registers application services and infrastructure with the dependency injection container.
    /// </summary>
    /// <param name="builder">The <see cref="WebApplicationBuilder"/> used to register services and configuration.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the required connection string named "vpr" is not found in configuration.
    /// </exception>
    /// <remarks>
    /// Services registered:
    /// - Logging factory with console provider and category-specific filters.
    /// - Entity Framework Core <see cref="VprContext"/> using SQL Server and debug-only logging/sensitive data options.
    /// - AutoMapper with a profile named <see cref="PickleBallProfile"/>.
    /// - ASP.NET Core controllers and endpoint metadata for minimal APIs.
    /// - Swagger/OpenAPI generation that includes XML comments and enables Swashbuckle annotations.
    /// 
    /// Important notes:
    /// - The connection string key expected is "vpr". Missing configuration will cause an <see cref="InvalidOperationException"/>.
    /// - In DEBUG builds EF Core logging is wired to the same logger factory and sensitive data logging is enabled;
    ///   remove or guard sensitive data logging for production environments.
    /// - Mapper configuration is registered as a singleton and validated at startup in DEBUG builds by <see cref="Main(string[])"/>.
    /// </remarks>
    public static void ConfigureServices(WebApplicationBuilder builder)
    {
        // Add services to the container.

        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
            // Set default minimum log level for all categories
            builder.SetMinimumLevel(LogLevel.Trace);
            // Set a specific log level for a category using a wildcard
            builder.AddFilter("Microsoft.EntityFrameworkCore.*", LogLevel.Information);
            builder.AddFilter("Microsoft.AspNetCore.*", LogLevel.Warning);
        });
        builder.Services.AddSingleton(loggerFactory);

        var conString = builder.Configuration.GetConnectionString("vpr") ??
             throw new InvalidOperationException("Connection string 'Vpr' not found.");
        builder.Services.AddDbContext<VprContext>(options =>
            options
                .UseSqlServer(conString)
#if DEBUG
                .UseLoggerFactory(loggerFactory)
                .EnableSensitiveDataLogging()
#endif
        );

        var configuration = new MapperConfiguration(cfg => cfg.AddProfile<PickleBallProfile>(), loggerFactory);
        builder.Services.AddSingleton<IMapper>(sp => new Mapper(configuration, sp.GetService));

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options => 
        {
            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            options.EnableAnnotations(); 
        });
    }
}