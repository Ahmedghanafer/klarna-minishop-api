using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace Minishop.Tests;

public class OpenApiCorsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public OpenApiCorsTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task OpenApi_Allows_Cors()
    {
        var client = _factory.CreateClient();
        using var request = new HttpRequestMessage(HttpMethod.Get, "/openapi/v1.json");
        request.Headers.Add("Origin", "https://example.com");

        using var response = await client.SendAsync(request);
        var allowedOrigin = response.Headers.TryGetValues("Access-Control-Allow-Origin", out var values)
            ? values.Single() : string.Empty;

        Assert.Equal("https://example.com", allowedOrigin);
    }
}