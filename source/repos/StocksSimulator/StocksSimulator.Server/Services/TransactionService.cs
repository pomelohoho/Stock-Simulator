using Microsoft.EntityFrameworkCore;
using StocksSimulator.Server.Data;
using StocksSimulator.Server.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace StocksSimulator.Server.Services
{
    public class TransactionService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TransactionService> _logger;

        public TransactionService(
            ApplicationDbContext context,
            ILogger<TransactionService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<TransactionResult> ExecuteTrade(int userId, int securityId, string type, int quantity)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Validate input parameters
                if (quantity <= 0)
                    return TransactionResult.CreateFailure("Quantity must be greater than zero");

                if (!new[] { "buy", "sell" }.Contains(type.ToLower()))
                    return TransactionResult.CreateFailure("Invalid transaction type");

                // Get user with holdings
                var user = await _context.Users
                    .Include(u => u.Holdings)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                    return TransactionResult.CreateFailure("User not found");

                // Get security with price
                var security = await _context.Securities
                    .FirstOrDefaultAsync(s => s.Id == securityId);

                if (security == null)
                    return TransactionResult.CreateFailure("Security not found");

                if (security.CurrentPrice <= 0)
                    return TransactionResult.CreateFailure("Invalid security price");

                var currentPrice = security.CurrentPrice;
                var totalAmount = quantity * currentPrice;

                // Process transaction type
                return type.ToLower() switch
                {
                    "buy" => await ProcessBuy(user, security, quantity, currentPrice, totalAmount),
                    "sell" => await ProcessSell(user, security, quantity, currentPrice, totalAmount),
                    _ => TransactionResult.CreateFailure("Invalid transaction type")
                };
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update error during trade execution");
                await transaction.RollbackAsync();
                return TransactionResult.CreateFailure($"Database error: {ex.InnerException?.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during trade execution");
                await transaction.RollbackAsync();
                return TransactionResult.CreateFailure($"Transaction failed: {ex.Message}");
            }
        }

        private async Task<TransactionResult> ProcessBuy(
            User user,
            Security security,
            int quantity,
            decimal currentPrice,
            decimal totalAmount)
        {
            if (user.Balance < totalAmount)
            {
                return TransactionResult.CreateFailure(
                    $"Insufficient funds. Needed: {totalAmount:C}, Available: {user.Balance:C}");
            }

            // Update user balance
            user.Balance -= totalAmount;

            // Get or create holding
            var holding = user.Holdings.FirstOrDefault(h => h.SecurityId == security.Id);
            if (holding == null)
            {
                holding = new Holding
                {
                    UserId = user.Id,
                    SecurityId = security.Id,
                    Quantity = 0,
                    AveragePrice = 0
                };
                _context.Holdings.Add(holding);
            }

            // Calculate new average price
            var totalCost = (holding.Quantity * holding.AveragePrice) + totalAmount;
            holding.Quantity += quantity;
            holding.AveragePrice = totalCost / holding.Quantity;

            // Record transaction
            RecordTransaction(user.Id, security.Id, "buy", quantity, currentPrice, totalAmount);

            await FinalizeTransaction();
            return TransactionResult.CreateSuccess("Buy order executed successfully");
        }

        private async Task<TransactionResult> ProcessSell(
            User user,
            Security security,
            int quantity,
            decimal currentPrice,
            decimal totalAmount)
        {
            var holding = user.Holdings.FirstOrDefault(h => h.SecurityId == security.Id);

            if (holding == null || holding.Quantity < quantity)
            {
                var available = holding?.Quantity ?? 0;
                return TransactionResult.CreateFailure(
                    $"Insufficient shares. Requested: {quantity}, Available: {available}");
            }

            // Update user balance
            user.Balance += totalAmount;

            // Update holding
            holding.Quantity -= quantity;
            if (holding.Quantity == 0)
            {
                _context.Holdings.Remove(holding);
            }

            // Record transaction
            RecordTransaction(user.Id, security.Id, "sell", quantity, currentPrice, totalAmount);

            await FinalizeTransaction();
            return TransactionResult.CreateSuccess("Sell order executed successfully");
        }

        private void RecordTransaction(
            int userId,
            int securityId,
            string type,
            int quantity,
            decimal pricePerShare,
            decimal totalAmount)
        {
            _context.Transactions.Add(new Transaction
            {
                UserId = userId,
                SecurityId = securityId,
                Type = type,
                Quantity = quantity,
                PricePerShare = pricePerShare,
                TotalAmount = totalAmount,
                TransactionDate = DateTime.UtcNow
            });
        }

        private async Task FinalizeTransaction()
        {
            await _context.SaveChangesAsync();
            await _context.Database.CurrentTransaction.CommitAsync();
        }
    }

    public record TransactionResult(bool IsSuccessful, string Message)
    {
        public static TransactionResult CreateSuccess(string message) => new(true, message);
        public static TransactionResult CreateFailure(string message) => new(false, message);
    }
}