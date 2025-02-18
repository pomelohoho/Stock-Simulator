using StocksSimulator.Server.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace StocksSimulator.Server.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public decimal Balance { get; set; } = 100000; // Initial balance
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public ICollection<Holding> Holdings { get; set; }
        public ICollection<Transaction> Transactions { get; set; }
    }
}
