using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Cors:AllowedOrigins:0, :1, ...
var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? Array.Empty<string>();

builder.Services.AddCors(o =>
{
    o.AddPolicy("AppCors", p => p
        .WithOrigins(allowedOrigins)   // exact match incl. scheme + port
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// IMPORTANT: order for endpoint routing + CORS
app.UseRouting();
app.UseCors("AppCors");   // global CORS (do NOT also use .RequireCors on endpoints)

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Minishop API v1"));
}

app.MapGet("/health", () =>
{
    var version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "0.1.0";
    return Results.Ok(new { status = "ok", version });
});

app.Run();
public partial class Program { }