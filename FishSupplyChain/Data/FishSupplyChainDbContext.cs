using FishSupplyChain.Data.Config;
using FishSupplyChain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FishSupplyChain.Data
{
    public class FishSupplyChainDbContext(DbContextOptions<FishSupplyChainDbContext> options) : DbContext (options)
    {
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<WalletEntity> Wallets { get; set; }
        public DbSet<RefreshTokenEntity> RefreshTokens { get; set; }
        public DbSet<EmailVerificationEntity> EmailVerifications { get; set; }
        public DbSet<PasswordResetTokenEntity> PasswordResetTokens { get; set; }
        public DbSet<FishFarmEntity> Farms { get; set; }
        public DbSet<FishPondEntity> Ponds { get; set; }
        public DbSet<SensorEntity> Sensors { get; set; }
        public DbSet<SensorReadingEntity> SensorReadings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserConfig());
            modelBuilder.ApplyConfiguration(new WalletConfig());
            modelBuilder.ApplyConfiguration(new FarmConfig());
            modelBuilder.ApplyConfiguration(new PondConfig());
            modelBuilder.ApplyConfiguration(new SensorConfig());
            modelBuilder.ApplyConfiguration(new SensorReadingConfig());
        }
    }
}
