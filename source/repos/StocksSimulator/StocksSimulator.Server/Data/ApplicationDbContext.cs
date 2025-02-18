using Microsoft.EntityFrameworkCore;
using StocksSimulator.Server.Models;

namespace StocksSimulator.Server.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Security> Securities { get; set; }
        public DbSet<PriceHistory> PriceHistories { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Holding> Holdings { get; set; }
        public DbSet<SimulationResult> SimulationResults { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PriceHistory>()
                .HasOne(ph => ph.Security)
                .WithMany(s => s.PriceHistories)
                .HasForeignKey(ph => ph.SecurityId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Holding>()
                .HasOne(h => h.User)
                .WithMany(u => u.Holdings)
                .HasForeignKey(h => h.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Holding>()
                .HasOne(h => h.Security)
                .WithMany(s => s.Holdings)
                .HasForeignKey(h => h.SecurityId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.User)
                .WithMany(u => u.Transactions)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Security)
                .WithMany()
                .HasForeignKey(t => t.SecurityId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Transaction>()
                .Property(t => t.PricePerShare)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Security>()
                .Property(s => s.CurrentPrice)
                .HasPrecision(18, 2);
        }
    }
}