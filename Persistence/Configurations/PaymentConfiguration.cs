using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
       public void Configure(EntityTypeBuilder<Payment> builder)
       {
              builder.HasKey(x => x.Id);

              builder.Property(x => x.AccountId)
                     .IsRequired();

              builder.Property(x => x.Amount)
                     .HasColumnType("decimal(18,2)")
                     .IsRequired();

              builder.Property(x => x.Currency)
                     .IsRequired()
                     .HasMaxLength(10);

              builder.Property(x => x.Description)
                     .HasMaxLength(250);

              builder.Property(x => x.Status)
                     .IsRequired();

              builder.Property(x => x.TransactionId)
                     .HasMaxLength(100)
                     .IsRequired(false);

              builder.Property(x => x.PaymentMethod)
                     .HasMaxLength(50)
                     .IsRequired(false);

              builder.HasOne(x => x.Account)
                     .WithMany()
                     .HasForeignKey(x => x.AccountId)
                     .OnDelete(DeleteBehavior.Cascade);
       }
}
