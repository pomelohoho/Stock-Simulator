using StocksSimulator.Server.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace StocksSimulator.Server.Models
{
    public class Holding
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int SecurityId { get; set; }
        public int Quantity { get; set; }
        public decimal AveragePrice { get; set; } // Average purchase price

        public User User { get; set; }
        public Security Security { get; set; }
    }
}