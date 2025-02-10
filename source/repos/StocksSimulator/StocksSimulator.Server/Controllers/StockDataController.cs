//using Microsoft.AspNetCore.Mvc;
//using System.Threading.Tasks;

//namespace StocksSimulator.Server.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public class StockDataController : ControllerBase
//    {
//        private readonly AlphaVantageService _alphaService;

//        public StockDataController(AlphaVantageService alphaService)
//        {
//            _alphaService = alphaService;
//        }

//        [HttpGet("intraday")]
//        public async Task<IActionResult> GetIntradayData(
//            [FromQuery] string symbol = "IBM",
//            [FromQuery] string interval = "5min")
//        {
//            // Replace with your real API key
//            string apiKey = "ZKSE2HHKC8NJ5F0G";

//            var data = await _alphaService.GetIntradayDataAsync(symbol, interval, apiKey);

//            if (data == null)
//            {
//                return BadRequest("No data returned from Alpha Vantage.");
//            }

//            return Ok(data);
//        }
//    }
//}
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

    // 1. Fetch from API and store in DB
    // GET /api/stockdata/store?symbol=IBM
    [HttpGet("store")]
    public async Task<IActionResult> StoreIntradayData([FromQuery] string symbol = "IBM")
    {
        // Replace with your real API key from Alpha Vantage
        string apiKey = "ZKSE2HHKC8NJ5F0G";

        var savedRecords = await _alphaService.FetchAndStoreIntradayAsync(symbol, apiKey);
        return Ok($"{savedRecords.Count} record(s) saved for {symbol}.");
    }

    // 2. Return the last 100 from DB
    // GET /api/stockdata/intraday/all?symbol=IBM
    [HttpGet("intraday/all")]
    public async Task<IActionResult> GetIntradayData([FromQuery] string symbol = "IBM")
    {
        var results = await _dbContext.StockPrices
            .Where(x => x.Symbol == symbol)
            .OrderByDescending(x => x.TimeStamp)
            .Take(100)
            .ToListAsync();

        return Ok(results);
    }
}
