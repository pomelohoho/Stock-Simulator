import React, { useEffect, useState } from "react";
import { getIntradayStockData } from "./services/stockService";

function StockList() {
    const [stocks, setStocks] = useState([]);
    const [symbol, setSymbol] = useState("IBM"); // Default symbol
    const [loading, setLoading] = useState(false);

    useEffect(() => {
        fetchStocks(symbol);
    }, [symbol]); // Runs when symbol changes

    async function fetchStocks(symbol) {
        setLoading(true);
        const data = await getIntradayStockData(symbol);
        setStocks(data);
        setLoading(false);
    }

    return (
        <div>
            <h2>Stock Data for {symbol}</h2>

            <label>
                Select Symbol:
                <select onChange={(e) => setSymbol(e.target.value)} value={symbol}>
                    <option value="IBM">IBM</option>
                    <option value="AAPL">Apple (AAPL)</option>
                    <option value="MSFT">Microsoft (MSFT)</option>
                </select>
            </label>

            {loading ? <p>Loading...</p> : (
                <table border="1">
                    <thead>
                        <tr>
                            <th>Time</th>
                            <th>Open</th>
                            <th>High</th>
                            <th>Low</th>
                            <th>Close</th>
                            <th>Volume</th>
                        </tr>
                    </thead>
                    <tbody>
                        {stocks.map((stock) => (
                            <tr key={stock.timeStamp}>
                                <td>{new Date(stock.timeStamp).toLocaleString()}</td>
                                <td>{stock.open}</td>
                                <td>{stock.high}</td>
                                <td>{stock.low}</td>
                                <td>{stock.close}</td>
                                <td>{stock.volume}</td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            )}
        </div>
    );
}

export default StockList;
