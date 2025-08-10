using FishSupplyChain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FishSupplyChain.Data.Config
{
    public class PondConfig : IEntityTypeConfiguration<FishPondEntity>
    {
        public void Configure(EntityTypeBuilder<FishPondEntity> builder)
        {
            builder.ToTable("Ponds");
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id).UseIdentityColumn();

            builder.Property(p => p.Name).IsRequired();

            builder.HasOne(p => p.FishFarm)
                   .WithMany(f => f.Ponds)
                   .HasForeignKey(p => p.FishFarmId)
                   .HasConstraintName("FK_Pond_FishFarm");
        }
    }
}
