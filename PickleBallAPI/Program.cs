using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PickleBallAPI.Models;
using System;

namespace PickleBallAPI;

public static class Program
{
    public static int Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        ConfigureServices(builder);
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
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
        builder.Services.AddSwaggerGen();
    }
}