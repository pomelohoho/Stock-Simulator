using StocksSimulator.Server.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace StocksSimulator.Server.Models
{
    public class SimulationResult
    {
        public int Id { get; set; }  // Primary Key
        public string AlgorithmName { get; set; }
        public decimal ProfitOrLoss { get; set; }
        public DateTime RunDate { get; set; }
    }
}