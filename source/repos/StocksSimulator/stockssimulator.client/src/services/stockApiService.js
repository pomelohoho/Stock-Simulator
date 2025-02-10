export async function getLocalIntradayData(symbol = "IBM") {
  const response = await fetch(`/api/stockdata/intraday/all?symbol=${symbol}`);
  if (!response.ok) {
    throw new Error("Failed to retrieve intraday data from the server");
  }
  return response.json(); // parse JSON
}
