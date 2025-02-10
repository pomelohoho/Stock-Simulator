const API_BASE_URL = "https://localhost:7155/api/stockdata"; // Ensure this matches your .NET backend

export async function getIntradayStockData(symbol) {
    try {
        const response = await fetch(`${API_BASE_URL}/intraday/all?symbol=${symbol}`);
        if (!response.ok) {
            throw new Error("Failed to fetch stock data");
        }
        return await response.json();
    } catch (error) {
        console.error("Error fetching stock data:", error);
        return [];
    }
}
