using System.Reflection;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Minishop.Application.Auth.Interfaces;
using Minishop.Application.Auth.Services;
using Minishop.Infrastructure.Auth;
using Minishop.Infrastructure.Persistence;
using Minishop.Infrastructure.Persistence.Seeding;

var builder = WebApplication.CreateBuilder(args);

// ---------- CORS ----------
var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? Array.Empty<string>();

builder.Services.AddCors(o =>
{
    o.AddPolicy("AppCors", p => p
        .WithOrigins(allowedOrigins)
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials());
});

// ---------- Swagger ----------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ---------- Database (EF Core) ----------
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// ---------- Controllers ----------
builder.Services.AddControllers();

// ---------- Auth DI (Clean Architecture) ----------
builder.Services.AddScoped<IUserRepository, UserRepositoryEf>();
builder.Services.AddScoped<IRefreshTokenStore, RefreshTokenStoreEf>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasherBCrypt>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// ---------- JWT Bearer ----------
var jwtSection = builder.Configuration.GetSection("Jwt");
var issuer = jwtSection["Issuer"] ?? "Minishop.Dev";
var audience = jwtSection["Audience"] ?? "Minishop.Web";
var key = jwtSection["Key"] ?? "dev-only-super-long-random-key-change-me";

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            ClockSkew = TimeSpan.FromSeconds(60),
            RoleClaimType = ClaimTypes.Role
        };
    });

builder.Services.AddAuthorization();

// ---------- Seeding (dev only) ----------
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddHostedService<ProductSeeder>();
}

var app = builder.Build();

// ---------- Middleware order ----------
app.UseRouting();
app.UseCors("AppCors");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Minishop API v1"));
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// keep your health endpoint
app.MapGet("/health", () =>
{
    var version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "0.1.0";
    return Results.Ok(new { status = "ok", version });
});

app.Run();

public partial class Program { }
