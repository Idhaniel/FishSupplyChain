using FishSupplyChain.Data.Config;
using FishSupplyChain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FishSupplyChain.Data
{
    public class FishSupplyChainDbContext(DbContextOptions<FishSupplyChainDbContext> options) : DbContext (options)
    {
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<WalletEntity> Wallets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserConfig());
            modelBuilder.ApplyConfiguration(new WalletConfig());
        }
    }
}
