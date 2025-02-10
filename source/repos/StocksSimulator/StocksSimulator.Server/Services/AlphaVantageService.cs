//using System.Net.Http;
//using System.Threading.Tasks;
//using System.Collections.Generic;
//using System.Text.Json;

//public class AlphaVantageService
//{
//    private readonly HttpClient _httpClient;

//    public AlphaVantageService(HttpClient httpClient)
//    {
//        _httpClient = httpClient;
//    }

//    public async Task<Dictionary<string, object>?> GetIntradayDataAsync(
//        string symbol,
//        string interval = "5min",
//        string apiKey = "ZKSE2HHKC8NJ5F0G")
//    {
//        string url = $"https://www.alphavantage.co/query?" +
//                     $"function=TIME_SERIES_INTRADAY&symbol={symbol}" +
//                     $"&interval={interval}&apikey=ZKSE2HHKC8NJ5F0G";

//        var response = await _httpClient.GetAsync(url);
//        response.EnsureSuccessStatusCode();

//        var content = await response.Content.ReadAsStringAsync();

//        var jsonOptions = new JsonSerializerOptions
//        {
//            PropertyNameCaseInsensitive = true
//        };


//        var data = JsonSerializer.Deserialize<Dictionary<string, object>>(content, jsonOptions);
//        return data;
//    }
//}

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using StocksSimulator.Server.Data;

public class AlphaVantageService
{
    private readonly HttpClient _httpClient;
    private readonly ApplicationDbContext _dbContext;

    public AlphaVantageService(HttpClient httpClient, ApplicationDbContext dbContext)
    {
        _httpClient = httpClient;
        _dbContext = dbContext;
    }

    public async Task<List<StockPrice>> FetchAndStoreIntradayAsync(string symbol, string apiKey)
    {
        // 1. Build URL
        // function=TIME_SERIES_INTRADAY, interval=5min, outputsize=compact for the last ~100 data points
        // Replace "5min" with another interval if desired, e.g. "15min" or "60min"
        string url = $"https://www.alphavantage.co/query?function=TIME_SERIES_INTRADAY" +
                     $"&symbol={symbol}&interval=5min&outputsize=compact&apikey={apiKey}";

        // 2. Fetch from Alpha Vantage
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        var jsonString = await response.Content.ReadAsStringAsync();

        // 3. Parse JSON
        var newRecords = ParseIntradayJson(symbol, jsonString);

        // 4. Insert new records
        foreach (var record in newRecords)
        {
            // check if we already have this symbol+timestamp
            bool exists = await _dbContext.StockPrices
                .AnyAsync(x => x.Symbol == symbol && x.TimeStamp == record.TimeStamp);

            if (!exists)
            {
                _dbContext.StockPrices.Add(record);
            }
        }

        await _dbContext.SaveChangesAsync();
        return newRecords;
    }

    private List<StockPrice> ParseIntradayJson(string symbol, string jsonString)
    {
        var results = new List<StockPrice>();

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        // top-level data is a dictionary
        var data = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonString, jsonOptions);
        if (data == null) return results;

        // The time series is usually under "Time Series (5min)"
        const string timeSeriesKey = "Time Series (5min)";
        if (!data.ContainsKey(timeSeriesKey))
        {
            // Possibly there's an error or a "Note" field 
            return results;
        }

        // timeSeriesElement is a JSON object containing many sub-objects
        var timeSeriesElement = (JsonElement)data[timeSeriesKey];

        // each property is something like "2023-05-10 15:30:00" => { "1. open": "...", etc. }
        foreach (var candleProp in timeSeriesElement.EnumerateObject())
        {
            string dateTimeString = candleProp.Name; // e.g. "2023-05-10 15:30:00"
            var candleJson = candleProp.Value;

            // parse each field
            var open = decimal.Parse(candleJson.GetProperty("1. open").GetString());
            var high = decimal.Parse(candleJson.GetProperty("2. high").GetString());
            var low = decimal.Parse(candleJson.GetProperty("3. low").GetString());
            var close = decimal.Parse(candleJson.GetProperty("4. close").GetString());
            var volume = long.Parse(candleJson.GetProperty("5. volume").GetString());

            var dt = DateTime.Parse(dateTimeString);

            results.Add(new StockPrice
            {
                Symbol = symbol,
                TimeStamp = dt,
                Open = open,
                High = high,
                Low = low,
                Close = close,
                Volume = volume
            });
        }

        return results;
    }
}

