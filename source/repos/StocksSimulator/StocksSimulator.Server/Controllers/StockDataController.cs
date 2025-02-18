using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using StocksSimulator.Server.Data;

[ApiController]
[Route("api/[controller]")]
public class StockDataController : ControllerBase
{
    private readonly AlphaVantageService _alphaService;
    private readonly ApplicationDbContext _dbContext;

    public StockDataController(AlphaVantageService alphaService, ApplicationDbContext dbContext)
    {
        _alphaService = alphaService;
        _dbContext = dbContext;
    }

    // 1️⃣ Fetch from API and Store in DB
    // GET /api/stockdata/store?symbol=IBM
    [HttpGet("store")]
    public async Task<IActionResult> StoreIntradayData([FromQuery] string symbol = "IBM")
    {
        // Replace with your real API key from Alpha Vantage
        string apiKey = "ZKSE2HHKC8NJ5F0G";

        var savedRecords = await _alphaService.FetchAndStoreIntradayAsync(symbol, apiKey);
        return Ok($"{savedRecords.Count} record(s) saved for {symbol}.");
    }

    // 2️⃣ Fetch the Last 100 Prices from the Database
    // GET /api/stockdata/intraday/all?symbol=IBM
    [HttpGet("intraday/all")]
    public async Task<IActionResult> GetIntradayData([FromQuery] string symbol = "IBM")
    {
        // Get the Security ID
        var security = await _dbContext.Securities.FirstOrDefaultAsync(s => s.Symbol == symbol);
        if (security == null)
        {
            return NotFound($"No security found for symbol {symbol}.");
        }

        // Query the last 100 prices
        var results = await _dbContext.PriceHistories
            .Where(ph => ph.SecurityId == security.Id)
            .OrderByDescending(ph => ph.TimeStamp)
            .Take(100)
            .ToListAsync();

        return Ok(results);
    }

    [HttpGet("overview")]
    public async Task<IActionResult> FetchCompanyOverview([FromQuery] string symbol = "IBM")
    {
        string apiKey = "ZKSE2HHKC8NJ5F0G";
        var security = await _alphaService.FetchAndStoreCompanyOverviewAsync(symbol, apiKey);

        if (security == null)
            return NotFound($"No overview data found for {symbol}.");

        return Ok($"Stored/Updated: {security.Name}, symbol={security.Symbol}, sector={security.Sector}.");
    }

    [HttpGet("search")]
    public IActionResult SearchSymbol(string symbol)
    {
        var security = _dbContext.Securities  
            .Include(s => s.PriceHistories)  
            .AsNoTracking() 
            .FirstOrDefault(s => s.Symbol == symbol);

        if (security == null) return NotFound();

        return Ok(new
        {
            symbol = security.Symbol,
            securityID = security.Id,
            companyName = security.Name,    
            latestPrice = security.CurrentPrice,  
            historicalData = security.PriceHistories
                .OrderBy(h => h.TimeStamp)  
                .Select(h => new {
                    timestamp = h.TimeStamp,  
                    start = h.Open,
                    high = h.High,
                    low = h.Low,
                    close = h.Close
                })
        });
    }


}
