using FishSupplyChain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FishSupplyChain.Data.Config
{
    public class SensorReadingConfig : IEntityTypeConfiguration<SensorReadingEntity>
    {
        public void Configure(EntityTypeBuilder<SensorReadingEntity> builder)
        {
            builder.ToTable("SensorReadings");
            builder.HasKey(sr => sr.Id);

            builder.Property(sr => sr.Id).UseIdentityColumn();

            builder.Property(sr => sr.Timestamp)
                   .IsRequired()
                   .HasColumnType("datetime2");

            builder.Property(sr => sr.Temperature)
                   .IsRequired()
                   .HasColumnType("decimal(5, 2)");

            builder.Property(sr => sr.PHLevel)
                   .IsRequired()
                   .HasColumnType("decimal(3, 2)");

            builder.Property(sr => sr.OxygenLevel)
                   .IsRequired()
                   .HasColumnType("decimal(5, 2)");

            builder.HasOne(sr => sr.Sensor)
                   .WithMany(s => s.Readings)
                   .HasForeignKey(sr => sr.SensorId)
                   .HasConstraintName("FK_SensorReading_Sensor");
        }
    }
}
