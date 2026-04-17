using AK.Products.API.Endpoints;
using AK.Products.API.Extensions;
using AK.Products.API.Middleware;
using AK.Products.Application.Extensions;
using AK.Products.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "AK.Products API", Version = "v1", Description = "AC Products Microservice - Men, Women & Kids Dress Collections" });
});

var app = builder.Build();

app.UseMiddleware<ExceptionHandlerMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "AK.Products API v1"));
    await app.SeedDatabaseAsync();
}

app.MapProductEndpoints();

app.Run();

public partial class Program { }
