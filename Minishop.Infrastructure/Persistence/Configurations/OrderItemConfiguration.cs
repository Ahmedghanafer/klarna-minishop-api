using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Minishop.Core.Entities;

namespace Minishop.Infrastructure.Persistence.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> b)
    {
        b.ToTable("OrderItems");
        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
            .ValueGeneratedOnAdd()
            .HasDefaultValueSql("NEWSEQUENTIALID()");

        b.Property(x => x.Qty)
            .IsRequired();

        b.Property(x => x.UnitPriceSEK)
            .IsRequired()
            .HasPrecision(18, 2);

        // Merge rule: exactly one line per (Order, Product)
        b.HasIndex(x => new { x.OrderId, x.ProductId })
            .HasDatabaseName("UX_OrderItems_Order_Product")
            .IsUnique();

        b.HasOne(x => x.Order)
            .WithMany(o => o.Items)
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        b.HasOne(x => x.Product)
            .WithMany(p => p.OrderItems)
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        // Defensive checks
        b.ToTable(t =>
        {
            t.HasCheckConstraint("CK_OrderItem_Qty_Positive", "[Qty] > 0");
            t.HasCheckConstraint("CK_OrderItem_UnitPrice_NonNegative", "[UnitPriceSEK] >= 0");
        });
    }
}