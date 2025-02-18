const STOCKDATA_BASE_URL = "https://localhost:7155/api/stockdata";
const USERS_BASE_URL = "https://localhost:7155/api/users";
const TRADE_BASE_URL = "https://localhost:7155/api/trade";
const HOLDINGS_BASE_URL = "https://localhost:7155/api/holdings";

export async function getIntradayStockData(symbol) {
    try {
        const response = await fetch(`${STOCKDATA_BASE_URL}/intraday/all?symbol=${symbol}`);
        if (!response.ok) {
            throw new Error("Failed to fetch stock data");
        }
        return await response.json();
    } catch (error) {
        console.error("Error fetching stock data:", error);
        return [];
    }
}

export async function fetchHoldings(userId) {
    try {
        const response = await fetch(`${HOLDINGS_BASE_URL}/${userId}`); 
        return await response.json();
    } catch (error) {
        console.error("Error fetching holdings:", error);
        return [];
    }
}

export async function executeTrade(tradeRequest) {
    try {
        console.log("[DEBUG] Trade Request Payload:", JSON.stringify(tradeRequest, null, 2)); 
        const response = await fetch(`${TRADE_BASE_URL}/execute`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Accept': 'application/json'
            },
            body: JSON.stringify({
                ...tradeRequest,
                type: tradeRequest.type.toLowerCase()
            })
        });

        if (!response.ok) {
            const errorData = await response.json();
            throw new Error(errorData.message || 'Trade execution failed');
        }

        return await response.json();
    } catch (error) {
        console.error("Trade error:", error.message);
        return {
            success: false,
            message: error.message || 'Network error'
        };
    }
}

export async function getUserBalance(userId) {
    try {
        const response = await fetch(`${USERS_BASE_URL}/${userId}`); // Remove /users
        return await response.json();
    } catch (error) {
        console.error("Balance fetch error:", error);
        return { balance: 0 };
    }
}


export async function searchStockSymbol(symbol) {
    try {
        const response = await fetch(`${STOCKDATA_BASE_URL}/search?symbol=${symbol}`);
        if (!response.ok) throw new Error("Symbol not found");
        return await response.json();
    } catch (error) {
        console.error("Search error:", error);
        return null;
    }
}