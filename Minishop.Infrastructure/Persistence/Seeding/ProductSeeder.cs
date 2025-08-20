using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Minishop.Core.Entities;

namespace Minishop.Infrastructure.Persistence.Seeding;

public class ProductSeeder : IHostedService
{
    private readonly IServiceProvider _sp;
    public ProductSeeder(IServiceProvider sp) => _sp = sp;

    public async Task StartAsync(CancellationToken ct)
    {
        using var scope = _sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        if (await db.Products.AnyAsync(ct)) return;

        db.Products.AddRange(
            new Product { Id = new Guid("11111111-1111-1111-1111-111111111111"), Name="Coffee Mug", PriceSEK=99m, Stock=50 },
            new Product { Id = new Guid("22222222-2222-2222-2222-222222222222"), Name="T-Shirt",    PriceSEK=199m, Stock=30 },
            new Product { Id = new Guid("33333333-3333-3333-3333-333333333333"), Name="Notebook",   PriceSEK=59m,  Stock=100 }
        );
        await db.SaveChangesAsync(ct);
    }

    public Task StopAsync(CancellationToken ct) => Task.CompletedTask;
}