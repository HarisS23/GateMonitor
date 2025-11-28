using GateMonitor.Models;
using Microsoft.EntityFrameworkCore;

namespace GateMonitor.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Worker> Workers { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<RfidScanRecord> RfidScanRecords { get; set; }
        public DbSet<RfidScanAction> RfidScanActions { get; set; }
        public DbSet<RfidCard> RfidCards { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .Property(u => u.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Worker>()
                .Property(w => w.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Worker>()
                .HasIndex(w => w.RfidUid)
                .IsUnique();

            modelBuilder.Entity<Worker>()
                .HasOne<RfidCard>()
                .WithOne()
                .HasForeignKey<Worker>(w => w.RfidUid);

            modelBuilder.Entity<RfidScanRecord>()
                .HasOne(r => r.Worker)
                .WithMany(w => w.RfidScanRecords)
                .HasForeignKey(r => r.WorkerId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<RfidScanRecord>()
                .Property(r => r.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<RfidScanRecord>()
                .HasOne<RfidCard>()              
                .WithMany()                       
                .HasForeignKey(r => r.RfidUid)    
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
