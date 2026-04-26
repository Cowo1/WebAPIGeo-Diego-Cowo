using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using WebAPIGeo.Modelo;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// 1. Extraer la cadena de conexión del appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 2. Registrar el DbContext con Pomelo MySQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        connectionString,
        new MySqlServerVersion(new Version(8, 0, 0)) // Especificar versión de servidor
    )
);

var app = builder.Build();

// Configurar logging detallado
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exceptionHandlerPathFeature = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerPathFeature>();
        var exception = exceptionHandlerPathFeature?.Error;

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = 500;

        await context.Response.WriteAsJsonAsync(new 
        { 
            error = exception?.Message,
            stackTrace = exception?.StackTrace,
            type = exception?.GetType().Name
        });
    });
});

app.MapOpenApi();
app.MapScalarApiReference(); // Accedes vía /scalar/v1

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
