using StocksSimulator.Server.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace StocksSimulator.Server.Models
{
    public class PriceHistory
    {
        public int Id { get; set; }  // Primary Key (PK)
        public int SecurityId { get; set; }  // Foreign Key (FK) to Securities
        public DateTime TimeStamp { get; set; }
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
        public long Volume { get; set; }

        [JsonIgnore]
        public Security Security { get; set; }
    }
}
