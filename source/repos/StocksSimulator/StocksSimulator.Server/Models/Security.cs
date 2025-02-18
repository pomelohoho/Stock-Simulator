using StocksSimulator.Server.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace StocksSimulator.Server.Models
{
    public class Security
    {
        public int Id { get; set; }
        public string Symbol { get; set; }
        public string? Name { get; set; }     // e.g. "International Business Machines"
        public string? Sector { get; set; }   // e.g. "TECHNOLOGY"
        public decimal CurrentPrice { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [JsonIgnore]
        public ICollection<PriceHistory> PriceHistories { get; set; }
        [JsonIgnore]
        public ICollection<Holding> Holdings { get; set; }
    }
}
