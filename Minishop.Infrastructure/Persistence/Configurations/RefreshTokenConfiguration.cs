using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Minishop.Core.Entities;

namespace Minishop.Infrastructure.Persistence.Configurations;

public sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> b)
    {
        b.ToTable("RefreshTokens");
        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
            .ValueGeneratedOnAdd()
            .HasDefaultValueSql("NEWSEQUENTIALID()");

        b.Property(x => x.TokenHash)
            .IsRequired()
            .HasMaxLength(88); // Base64 of 32 bytes SHA-256 = 44 chars; room to grow

        b.Property(x => x.CreatedAtUtc)
            .HasDefaultValueSql("GETUTCDATE()")
            .IsRequired();

        b.Property(x => x.ExpiresAtUtc).IsRequired();

        b.HasIndex(x => x.TokenHash).IsUnique()
            .HasDatabaseName("UX_RefreshTokens_TokenHash");

        b.HasOne<User>()
            .WithMany() // we don't need a nav collection on User
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}