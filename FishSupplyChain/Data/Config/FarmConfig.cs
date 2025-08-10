using FishSupplyChain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace FishSupplyChain.Data.Config
{
    public class FarmConfig : IEntityTypeConfiguration<FishFarmEntity>
    {
        public void Configure(EntityTypeBuilder<FishFarmEntity> builder)
        {
            builder.ToTable("Farms");
            builder.HasKey(f => f.Id);

            builder.Property(f => f.Id).UseIdentityColumn();

            builder.Property(f => f.Name).IsRequired();
            builder.Property(f => f.Location).IsRequired();
            builder.HasOne(f => f.User)
                   .WithMany(u => u.Farms)  // A user can have zero or more farms
                   .HasForeignKey(f => f.UserId)
                   .IsRequired();
        }
    }
}