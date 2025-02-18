using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StocksSimulator.Server.Data;
using StocksSimulator.Server.Models.DTOs;
using StocksSimulator.Server.Services;

namespace StocksSimulator.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TradeController : ControllerBase
    {
        private readonly TransactionService _transactionService;
        private readonly ApplicationDbContext _context;

        public TradeController(
            TransactionService transactionService,
            ApplicationDbContext context)
        {
            _transactionService = transactionService;
            _context = context;
        }

        [HttpPost("execute")]
        public async Task<IActionResult> ExecuteTrade([FromBody] TradeRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "Invalid request",
                    Errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                });
            }

            var result = await _transactionService.ExecuteTrade(
                request.UserId,
                request.SecurityId,
                request.Type.ToLower(),
                request.Quantity
            );

            // Use the IsSuccessful property instead of the factory method
            return result.IsSuccessful ? Ok(result) : BadRequest(result);
        }

        [HttpGet("history/{userId}")]
        public async Task<IActionResult> GetTransactionHistory(int userId)
        {
            var history = await _context.Transactions
                .Include(t => t.Security)
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.TransactionDate)
                .Select(t => new TransactionDto
                {
                    Symbol = t.Security.Symbol,
                    Type = t.Type,
                    Quantity = t.Quantity,
                    Price = t.PricePerShare,
                    Total = t.TotalAmount,
                    Date = t.TransactionDate
                })
                .ToListAsync();

            return Ok(history);
        }
    }
}