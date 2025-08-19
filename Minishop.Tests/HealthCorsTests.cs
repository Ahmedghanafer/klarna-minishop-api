// file: HealthCorsTests.cs
using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Minishop.Tests;

public class HealthCorsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public HealthCorsTests(WebApplicationFactory<Program> factory)
    {
        // Inject the same allowed-origins your app uses
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((_, config) =>
            {
                var dict = new Dictionary<string, string?>
                {
                    ["Cors:AllowedOrigins:0"] = "http://localhost:5173",
                    ["Cors:AllowedOrigins:1"] = "https://app.mydomain.com"
                };
                config.AddInMemoryCollection(dict!);
            });
        });
    }

    [Theory]
    [InlineData("http://localhost:5173", true)]
    [InlineData("https://app.mydomain.com", true)]
    [InlineData("https://evil.example", false)]
    public async Task Health_GlobalCors_MatchesPolicy(string origin, bool shouldAllow)
    {
        var client = _factory.CreateClient();

        using var req = new HttpRequestMessage(HttpMethod.Get, "/health");
        req.Headers.TryAddWithoutValidation("Origin", origin);

        var res = await client.SendAsync(req);

        Assert.Equal(HttpStatusCode.OK, res.StatusCode);

        var has = res.Headers.TryGetValues("Access-Control-Allow-Origin", out var vals);
        var value = vals?.SingleOrDefault();

        if (shouldAllow)
        {
            Assert.True(has);
            Assert.Equal(origin, value);
        }
        else
        {
            Assert.False(has); // disallowed origin => header should be absent
        }
    }
}