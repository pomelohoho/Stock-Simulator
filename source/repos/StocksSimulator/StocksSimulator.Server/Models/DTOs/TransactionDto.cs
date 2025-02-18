using System;

namespace StocksSimulator.Server.Models.DTOs
{
    public class TransactionDto
    {
        public string Symbol { get; set; }
        public string Type { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get; set; }
        public DateTime Date { get; set; }
    }
}