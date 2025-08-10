using FishSupplyChain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FishSupplyChain.Data.Config
{
    public class SensorConfig : IEntityTypeConfiguration<SensorEntity>
    {
        public void Configure(EntityTypeBuilder<SensorEntity> builder)
        {
            builder.ToTable("Sensors");
            builder.HasKey(s => s.Id);

            builder.Property(s => s.Id).UseIdentityColumn();

            builder.HasOne(s => s.Pond)
                   .WithOne(p => p.Sensor)
                   .HasForeignKey<SensorEntity>(s => s.PondId)
                   .HasConstraintName("FK_Sensor_FishPond");
        }
    }
}
