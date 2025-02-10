using Microsoft.EntityFrameworkCore;
using System;

namespace StocksSimulator.Server.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<StockPrice> StockPrices { get; set; }
        public DbSet<SimulationResult> SimulationResults { get; set; }
    }

    public class StockPrice
    {
        public int Id { get; set; }
        public string Symbol { get; set; }
        public DateTime TimeStamp { get; set; }
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
        public long Volume { get; set; }
    }

    public class SimulationResult
    {
        public int Id { get; set; }
        public string AlgorithmName { get; set; }
        public decimal ProfitOrLoss { get; set; }
        public DateTime RunDate { get; set; }
    }
}
