using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.AccountType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Currency)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(x => x.Balance)
            .HasColumnType("decimal(18,2)");

        builder.Property(x => x.IsActive)
            .HasDefaultValue(true);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
