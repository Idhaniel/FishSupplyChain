using FishSupplyChain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FishSupplyChain.Data.Config
{
    public class WalletConfig : IEntityTypeConfiguration<WalletEntity>
    {
        public void Configure(EntityTypeBuilder<WalletEntity> builder)
        {
            builder.ToTable("Wallets");
            builder.HasKey(w => w.Id);

            builder.Property(w => w.Id).UseIdentityColumn();

            builder.Property(w => w.Address).HasMaxLength(42).IsRequired();
            builder.Property(w => w.PrivateKey).HasColumnType("nvarchar(max)");
            builder.Property(w => w.SeedPhrase).HasColumnType("nvarchar(max)");

            builder.HasOne(w => w.User)
                   .WithOne(u => u.Wallet)
                   .HasForeignKey<WalletEntity>(w => w.UserId)
                   .HasConstraintName("FK_Wallet_User");
        }
    }
}
