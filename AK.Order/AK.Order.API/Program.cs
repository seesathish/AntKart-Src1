using AK.BuildingBlocks.HealthChecks;
using AK.BuildingBlocks.Logging;
using AK.Order.API.Endpoints;
using AK.Order.API.Middleware;
using AK.Order.Application.Extensions;
using AK.Order.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddSerilogLogging();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddDefaultHealthChecks();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "AK.Order API", Version = "v1", Description = "AntKart Order Microservice — PostgreSQL-backed order management" });
});

var app = builder.Build();

await app.ApplyMigrationsAsync();

app.UseMiddleware<ExceptionHandlerMiddleware>();

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "AK.Order API v1"));

app.MapOrderEndpoints();
app.MapDefaultHealthChecks();

app.Run();

public partial class Program { }
