using FinAxisLeaseBudgeting.Models;
using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;

namespace FinAxisLeaseBudgeting.Data
{
    public class FinAxisDbContext : DbContext
    {
        public FinAxisDbContext(DbContextOptions<FinAxisDbContext> options)
            : base(options)
        {
        }

        // Register our 5 Yardi staging tables
        public DbSet<CommChargeSchedule> CommChargeSchedules { get; set; } = null!;
        public DbSet<CommContact> CommContacts { get; set; } = null!;
        public DbSet<CommCustomer> CommCustomers { get; set; } = null!;
        public DbSet<CommLeaseUnit> CommLeaseUnits { get; set; } = null!;
        public DbSet<CommLease> CommLeases { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure strict financial precision for numeric columns (18,2)
            modelBuilder.Entity<CommChargeSchedule>(entity =>
            {
                entity.Property(e => e.Price).HasPrecision(18, 2);
                entity.Property(e => e.Quantity).HasPrecision(18, 2);
                entity.Property(e => e.Amount).HasPrecision(18, 2);
                entity.Property(e => e.AmountPeriod).HasPrecision(18, 2);
                entity.Property(e => e.MgmtFeePercentage).HasPrecision(10, 2);
                entity.Property(e => e.SalesTaxPercentage).HasPrecision(10, 2);
                entity.Property(e => e.AreaColumnOverride).HasPrecision(18, 2);
                entity.Property(e => e.EstimatedRent).HasPrecision(18, 2);
            });

            modelBuilder.Entity<CommLease>(entity =>
            {
                entity.Property(e => e.HoldoverPercentage).HasPrecision(10, 2);
                entity.Property(e => e.ContractedArea).HasPrecision(18, 2);
            });
        }
    }
}
