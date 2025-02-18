// TradeRequest.cs
using System.ComponentModel.DataAnnotations;

namespace StocksSimulator.Server.Models.DTOs
{
    public class TradeRequest
    {
        [Required(ErrorMessage = "User ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid User ID")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Security ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid Security ID")]
        public int SecurityId { get; set; }

        [Required(ErrorMessage = "Type is required")]
        [RegularExpression("buy|sell", ErrorMessage = "Type must be 'buy' or 'sell'")]
        public string Type { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }
    }
}