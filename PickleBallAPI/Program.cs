using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PickleBallAPI.Controllers;
using PickleBallAPI.Models;
using System;
using AutoMapper;
using System.Configuration;
using Humanizer.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Reflection;
using PickleBallAPI;
using Microsoft.Build.Logging;

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

        var conString = builder.Configuration.GetConnectionString("vpr") ??
             throw new InvalidOperationException("Connection string 'Vpr' not found.");
        builder.Services.AddDbContext<VprContext>(options =>
        options.UseSqlServer(conString));

        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
        });
        builder.Services.AddSingleton(loggerFactory);

        var configuration = new MapperConfiguration(cfg => cfg.AddProfile<PickleBallProfile>(), loggerFactory);
        builder.Services.AddSingleton<IMapper>(sp => new Mapper(configuration, sp.GetService));

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
    }
}