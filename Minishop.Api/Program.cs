using System.Reflection;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});



var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseCors();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseHttpsRedirection();



app.MapGet("/health", () =>
    {
        var version = Assembly.GetExecutingAssembly()
            .GetName().Version?.ToString() ?? "0.1.0";
        return Results.Ok(new { status = "ok", version });
    });

app.Run();
public partial class Program { }