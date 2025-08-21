using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Minishop.Core.Entities;

namespace Minishop.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> b)
    {
        b.ToTable("Products");
        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
            .ValueGeneratedOnAdd()
            .HasDefaultValueSql("NEWSEQUENTIALID()");

        b.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        b.Property(x => x.PriceSEK)
            .IsRequired()
            .HasPrecision(18, 2);

        b.Property(x => x.Stock)
            .IsRequired();

        // Defensive checks
        b.ToTable(t =>
        {
            t.HasCheckConstraint("CK_Product_Price_NonNegative", "[PriceSEK] >= 0");
            t.HasCheckConstraint("CK_Product_Stock_NonNegative", "[Stock] >= 0");
        });

        b.HasMany(x => x.OrderItems)
            .WithOne(oi => oi.Product)
            .HasForeignKey(oi => oi.ProductId)
            .OnDelete(DeleteBehavior.Restrict); // don’t allow deleting referenced products

        // Helpful index if you search by name
        b.HasIndex(x => x.Name).HasDatabaseName("IX_Products_Name");
    }
}
