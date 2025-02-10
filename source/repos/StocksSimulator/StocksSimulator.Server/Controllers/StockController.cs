using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StocksSimulator.Server.Data; 

namespace StocksSimulator.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StockController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        // Inject the DbContext in the constructor
        public StockController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllStocks()
        {
            // Query the StockPrices table in the database
            var allStocks = await _db.StockPrices.ToListAsync();
            return Ok(allStocks);
        }

        //[HttpGet("seed")]
        //public async Task<IActionResult> SeedData()
        //{
        //    var newRecord = new StockPrice
        //    {
        //        Symbol = "AAPL",
        //        TimeStamp = DateTime.Now,
        //        Price = 150.25m
        //    };

        //    _db.StockPrices.Add(newRecord);
        //    await _db.SaveChangesAsync();
        //    return Ok("Seed complete");
        //}

    }
}
