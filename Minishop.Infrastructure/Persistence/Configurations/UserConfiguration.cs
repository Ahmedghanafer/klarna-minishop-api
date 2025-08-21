using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Minishop.Core.Entities;

namespace Minishop.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> b)
    {
        b.ToTable("Users");
        b.HasKey(x => x.Id);

        // Prefer DB-generated sequential GUIDs (still ok if you set Id in code)
        b.Property(x => x.Id)
            .ValueGeneratedOnAdd()
            .HasDefaultValueSql("NEWSEQUENTIALID()");

        b.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(256);

        // Case-insensitive uniqueness via persisted computed column
        b.Property<string>("NormalizedEmail")
            .HasMaxLength(256)
            .HasColumnType("nvarchar(256)")
            .HasComputedColumnSql("UPPER([Email])", stored: true).IsRequired();

        b.HasIndex("NormalizedEmail")
            .HasDatabaseName("UX_Users_NormalizedEmail")
            .IsUnique();

        b.Property(x => x.PasswordHash)
            .IsRequired()
            .HasMaxLength(512); // room for strong hashes (e.g., Argon2/bcrypt)

        b.Property(x => x.Role)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        // A user can have many orders; don’t cascade delete users with orders
        b.HasMany(x => x.Orders)
            .WithOne(o => o.User)
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}