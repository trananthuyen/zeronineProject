using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using zeronineProject.Core.Entities;

namespace zeronineProject.Infrastructure.ExternalAPICalls.BinanceAPIs
{
    public class GetCandles
    {
        private readonly HttpClient _httpClient;

        public GetCandles(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Candle>> GetCandlesAsync(
            string symbol,
            string interval,
            int limit,
            CancellationToken cancellationToken = default)
        {
            symbol = symbol.Trim().ToUpperInvariant();

            var url =
                $"api/v3/klines" +
                $"?symbol={symbol}" +
                $"&interval={Uri.EscapeDataString(interval)}" +
                $"&limit={limit}";

            using var response = await _httpClient.GetAsync(
                url,
                cancellationToken);

            response.EnsureSuccessStatusCode();

            await using var stream =
                await response.Content.ReadAsStreamAsync(cancellationToken);

            var data = await JsonSerializer.DeserializeAsync<
                List<JsonElement[]>>(
                    stream,
                    cancellationToken: cancellationToken)
                ?? [];

            return data.Select(item => new Candle
            {
                Symbol = symbol,
                Interval = interval,

                OpenTime = DateTimeOffset
                    .FromUnixTimeMilliseconds(item[0].GetInt64())
                    .UtcDateTime,

                Open = ParseDecimal(item[1]),
                High = ParseDecimal(item[2]),
                Low = ParseDecimal(item[3]),
                Close = ParseDecimal(item[4]),
                Volume = ParseDecimal(item[5]),

                CloseTime = DateTimeOffset
                    .FromUnixTimeMilliseconds(item[6].GetInt64())
                    .UtcDateTime,

                QuoteAssetVolume = ParseDecimal(item[7]),
                NumberOfTrades = item[8].GetInt32(),
                TakerBuyBaseVolume = ParseDecimal(item[9]),
                TakerBuyQuoteVolume = ParseDecimal(item[10])
            }).ToList();
        }

        private static decimal ParseDecimal(JsonElement value)
        {
            return decimal.Parse(
                value.GetString()!,
                CultureInfo.InvariantCulture);
        }
    }
}
