using FinAxisLeaseBudgeting.Models;
using Microsoft.EntityFrameworkCore;
using PlanningAPI.Models;
using System.Reflection.Emit;

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
        public DbSet<UnitMaster> UnitMasters { get; set; }
        public DbSet<LeaseMaster> LeaseMasters { get; set; }
        public DbSet<PropertyMaster> PropertyMasters { get; set; }
        public DbSet<LeaseCharge> LeaseCharges { get; set; }

        public DbSet<EntityMaster> EntityMasters { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserLoginHistory> UserLoginHistories { get; set; } = null!;

        public DbSet<AuditTable> AuditTables { get; set; }

        public DbSet<Role> Roles => Set<Role>();
        public DbSet<RoleScreenPermission> RoleScreenPermissions => Set<RoleScreenPermission>();
        public DbSet<RoleFieldPermission> RoleFieldPermissions => Set<RoleFieldPermission>();
        public DbSet<UserScreenPermission> UserScreenPermissions => Set<UserScreenPermission>();
        public DbSet<UserFieldPermission> UserFieldPermissions => Set<UserFieldPermission>();
        public DbSet<ReportGroup> ReportGroups { get; set; }
        public DbSet<ReportGroupReportMapping> ReportGroupReportMappings { get; set; }

        public DbSet<PlLeaseBudget> PlLeaseBudgets { get; set; }

        public DbSet<PlLeaseBudgetDetail> PlLeaseBudgetDetails { get; set; }
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
            modelBuilder.Entity<User>()
                .HasOne(u => u.UserRole)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            // Unique index
            modelBuilder.Entity<Role>()
                .HasIndex(r => r.RoleName)
                .IsUnique()
                .HasDatabaseName("ux_roles_role_name");

            // Composite Keys
            modelBuilder.Entity<RoleScreenPermission>()
                .HasKey(x => new { x.RoleId, x.ScreenCode });

            modelBuilder.Entity<RoleFieldPermission>()
                .HasKey(x => new { x.RoleId, x.FieldCode });

            modelBuilder.Entity<UserScreenPermission>()
                .HasKey(x => new { x.UserId, x.ScreenCode });

            modelBuilder.Entity<UserFieldPermission>()
                .HasKey(x => new { x.UserId, x.FieldCode });

            // Delete Behaviours
            modelBuilder.Entity<User>()
                .HasOne(x => x.UserRole)
                .WithMany(r => r.Users)
                .HasForeignKey(x => x.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RoleScreenPermission>()
                .HasOne(x => x.Role)
                .WithMany(r => r.ScreenPermissions)
                .HasForeignKey(x => x.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RoleFieldPermission>()
                .HasOne(x => x.Role)
                .WithMany(r => r.FieldPermissions)
                .HasForeignKey(x => x.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserScreenPermission>()
                .HasOne(x => x.User)
                .WithMany(u => u.ScreenOverrides)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserFieldPermission>()
                .HasOne(x => x.User)
                .WithMany(u => u.FieldOverrides)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ReportGroupReportMapping>()
                .HasKey(x => new
                {
                    x.ReportGroupId,
                    x.ReportCode
                });

            modelBuilder.Entity<ReportGroupReportMapping>()
                .HasOne(x => x.ReportGroup)
                .WithMany(x => x.Reports)
                .HasForeignKey(x => x.ReportGroupId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<PlLeaseBudget>()
                .HasMany(x => x.Details)
                .WithOne(x => x.Budget)
                .HasForeignKey(x => x.BudgetId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
