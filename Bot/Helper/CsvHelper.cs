using CsvHelper;
using System.Globalization;
using System.Net;
using System.Net.Http;

namespace Bot.Helper
{
    public static class CsvHelper
    {
        public static async Task PostStockQuoteInformationToAllChatRooms(string chatUrl, string stockCode)
        {
            var url = $"https://stooq.com/q/l/?s={stockCode}&f=sd2t2ohlcv&h&e=csv";
            var botMessage = await GetBotMessage(url);
            await PostBotMessage(chatUrl, botMessage);
        }

        private static async Task<Stream> GetCsvFile(string url)
        {
            try
            {
                using var httpClient = new HttpClient();
                var req = await httpClient.GetAsync(url);
                return await req.Content.ReadAsStreamAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Request error", ex);
            }
        }

        private static async Task<Stock?> GetStockData(string url)
        {
            var stream = GetCsvFile(url);

            using (var reader = new StreamReader(await stream))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var record = csv.GetRecords<Stock>().First();
                if (record.Open != "N/D")
                {
                    return record;
                }
                return null;
            }
        }

        private static async Task<string> GetBotMessage(string url)
        {
            var stock = await GetStockData(url);
            if (stock is null)
            {
                throw new Exception("Quote not found!");
            }
            return $"{stock.Symbol} quote is ${stock.Open} per share";
        }

        private static async Task PostBotMessage(string chatUrl, string message)
        {
            var values = new Dictionary<string, string>
            {
                { "message", message },
            };
            
            var client = new HttpClient();
            
            var content = new FormUrlEncodedContent(values);

            await client.PostAsync(chatUrl, content);
        }


    }

    internal class Stock
    {
        public string Symbol { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string Open { get; set; }
        public string High { get; set; }
        public string Low { get; set; }
        public string Close { get; set; }
        public string Volume { get; set; }
    }
}
