using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Minishop.Core.Entities;

namespace Minishop.Infrastructure.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> b)
    {
        b.ToTable("Orders");
        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
            .ValueGeneratedOnAdd()
            .HasDefaultValueSql("NEWSEQUENTIALID()");

        b.Property(x => x.TotalSEK)
            .IsRequired()
            .HasPrecision(18, 2);

        b.Property(x => x.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        b.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        b.HasOne(x => x.User)
            .WithMany(u => u.Orders)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasMany(x => x.Items)
            .WithOne(i => i.Order)
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // Efficient queries by user, newest first (DESC applied at query time)
        b.HasIndex(x => new { x.UserId, x.CreatedAt })
            .HasDatabaseName("IX_Orders_User_CreatedAt");

        // Defensive check
        b.ToTable(t =>
        {
            t.HasCheckConstraint("CK_Order_Total_NonNegative", "[TotalSEK] >= 0");
        });
    }
}
