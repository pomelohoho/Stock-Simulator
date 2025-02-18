using StocksSimulator.Server.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace StocksSimulator.Server.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int SecurityId { get; set; }
        public string Type { get; set; } // "buy" or "sell"
        public int Quantity { get; set; }
        public decimal PricePerShare { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

        [JsonIgnore]
        public User User { get; set; }
        [JsonIgnore]
        public Security Security { get; set; }
    }
}