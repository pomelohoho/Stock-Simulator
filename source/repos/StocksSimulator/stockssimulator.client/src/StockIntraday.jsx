import React, { useEffect, useState } from "react";
import { getLocalIntradayData } from "./services/stockApiService";

function StockIntraday() {
    const [symbol, setSymbol] = useState("IBM");
    const [rows, setRows] = useState([]);

    useEffect(() => {
        async function loadData() {
            try {
                const data = await getLocalIntradayData(symbol);
                setRows(data);
            } catch (error) {
                console.error(error);
            }
        }
        loadData();
    }, [symbol]);

    return (
        <div>
            <h2>Local Intraday Data for {symbol}</h2>
            <input
                type="text"
                value={symbol}
                onChange={(e) => setSymbol(e.target.value)}
                placeholder="Enter Symbol (e.g. IBM)"
            />
            {rows.length === 0 ? (
                <p>No records found.</p>
            ) : (
                <ul>
                    {rows.map((r) => (
                        <li key={r.id}>
                            {r.timeStamp} - O:{r.open} H:{r.high} L:{r.low} C:{r.close} (Vol: {r.volume})
                        </li>
                    ))}
                </ul>
            )}
        </div>
    );
}

export default StockIntraday;
