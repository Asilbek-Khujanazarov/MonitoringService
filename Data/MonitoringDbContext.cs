using Microsoft.EntityFrameworkCore;
using PatientRecovery.MonitoringService.Models;

namespace PatientRecoverySystem.MonitoringService.Data
{
    public class MonitoringDbContext : DbContext
    {
        public MonitoringDbContext(DbContextOptions<MonitoringDbContext> options)
            : base(options)
        {
        }

        public DbSet<VitalSignsMonitor> VitalSignsMonitors { get; set; }
        public DbSet<VitalSignsAlert> VitalSignsAlerts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<VitalSignsMonitor>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasMany(e => e.Alerts)
                    .WithOne(e => e.Monitor)
                    .HasForeignKey(e => e.MonitorId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<VitalSignsAlert>(entity =>
            {
                entity.HasKey(e => e.Id);
            });
        }
    }
}