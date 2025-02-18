using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using StocksSimulator.Server.Data;
using StocksSimulator.Server.Models;
public class AlphaVantageService
{
    private readonly HttpClient _httpClient;
    private readonly ApplicationDbContext _dbContext;

    public AlphaVantageService(HttpClient httpClient, ApplicationDbContext dbContext)
    {
        _httpClient = httpClient;
        _dbContext = dbContext;
    }

    public async Task<Security> FetchAndStoreCompanyOverviewAsync(string symbol, string apiKey)
    {
        // 1. Build URL for the OVERVIEW endpoint
        string url = $"https://www.alphavantage.co/query?function=OVERVIEW&symbol={symbol}&apikey={apiKey}";

        // 2. Fetch JSON
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        var jsonString = await response.Content.ReadAsStringAsync();

        // 3. Parse
        var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var data = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonString, jsonOptions);

        if (data == null || !data.ContainsKey("Symbol") || !data.ContainsKey("Name") || !data.ContainsKey("Sector"))
        {
            // The API might return a "Note" about usage, or invalid symbol
            return null;
        }

        // 4. Extract needed fields
        string overviewSymbol = data["Symbol"];     // e.g. "IBM"
        string overviewName   = data["Name"];       // e.g. "International Business Machines"
        string overviewSector = data["Sector"];     // e.g. "TECHNOLOGY"

        // 5. Check if Security already exists
        var security = await _dbContext.Securities
            .FirstOrDefaultAsync(s => s.Symbol == overviewSymbol);

        if (security == null)
        {
            // Create new security
            security = new Security
            {
                Symbol       = overviewSymbol,
                Name         = overviewName,
                Sector       = overviewSector,
                CurrentPrice = 0
            };
            _dbContext.Securities.Add(security);
        }
        else
        {
            // Update existing security
            security.Name   = overviewName;
            security.Sector = overviewSector;
        }

        await _dbContext.SaveChangesAsync();
        return security;
    }

    public async Task<List<PriceHistory>> FetchAndStoreIntradayAsync(string symbol, string apiKey)
    {
        // 1. Build API URL
        string url = $"https://www.alphavantage.co/query?function=TIME_SERIES_INTRADAY" +
                     $"&symbol={symbol}&interval=5min&outputsize=compact&apikey={apiKey}";

        // 2. Fetch data from Alpha Vantage
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        var jsonString = await response.Content.ReadAsStringAsync();

        // 3. Parse JSON
        var newRecords = ParseIntradayJson(symbol, jsonString);

        // 4. Check if security exists in `Securities`, if not, create it
        var security = await _dbContext.Securities.FirstOrDefaultAsync(s => s.Symbol == symbol);
        if (security == null)
        {
            security = new Security { Symbol = symbol, CurrentPrice = 0 };
            _dbContext.Securities.Add(security);
            await _dbContext.SaveChangesAsync(); // Save to get the security ID
        }

        // 5. Update current price
        if (newRecords.Count > 0)
        {
            security.CurrentPrice = newRecords[0].Close;
            _dbContext.Securities.Update(security);
        }

        // 6. Insert new records into `PriceHistory`
        foreach (var record in newRecords)
        {
            // Ensure this exact timestamp doesn't already exist
            bool exists = await _dbContext.PriceHistories
                .AnyAsync(x => x.SecurityId == security.Id && x.TimeStamp == record.TimeStamp);

            if (!exists)
            {
                record.SecurityId = security.Id;
                _dbContext.PriceHistories.Add(record);
            }
        }

        await _dbContext.SaveChangesAsync();
        return newRecords;
    }

    private List<PriceHistory> ParseIntradayJson(string symbol, string jsonString)
    {
        var results = new List<PriceHistory>();

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        // Deserialize JSON data
        var data = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonString, jsonOptions);
        if (data == null) return results;

        // Alpha Vantage stores the time series data under "Time Series (5min)"
        const string timeSeriesKey = "Time Series (5min)";
        if (!data.ContainsKey(timeSeriesKey)) return results;

        var timeSeriesElement = (JsonElement)data[timeSeriesKey];

        foreach (var candleProp in timeSeriesElement.EnumerateObject())
        {
            string dateTimeString = candleProp.Name;
            var candleJson = candleProp.Value;

            results.Add(new PriceHistory
            {
                TimeStamp = DateTime.Parse(dateTimeString),
                Open = decimal.Parse(candleJson.GetProperty("1. open").GetString()),
                High = decimal.Parse(candleJson.GetProperty("2. high").GetString()),
                Low = decimal.Parse(candleJson.GetProperty("3. low").GetString()),
                Close = decimal.Parse(candleJson.GetProperty("4. close").GetString()),
                Volume = long.Parse(candleJson.GetProperty("5. volume").GetString())
            });
        }

        return results;
    }
}
