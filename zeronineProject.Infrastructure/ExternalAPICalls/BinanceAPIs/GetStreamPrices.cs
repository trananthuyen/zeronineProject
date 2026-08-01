using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using zeronineProject.Core.Entities;
using zeronineProject.Infrastructure.ExternalAPICalls.TelegramAPIs;

namespace zeronineProject.Infrastructure.ExternalAPICalls.BinanceAPIs
{
    public class GetStreamPrices
    {
        private readonly SendMessage _sendMessage;
        private readonly IConfiguration _configuration;

        private readonly Dictionary<string, DateTime>
        _lastSendTimes =
            new(StringComparer.OrdinalIgnoreCase);

        private readonly TimeSpan _interval =
            TimeSpan.FromSeconds(30);

        public GetStreamPrices(SendMessage sendMessage, IConfiguration configuration)
        {
            _sendMessage = sendMessage;
            _configuration = configuration;
        }

        public async IAsyncEnumerable<TradingPair> StreamPricesAsync(
        IEnumerable<string> symbols, 
        [EnumeratorCancellation]
        CancellationToken cancellationToken)
        {
            var normalizedSymbols = symbols
                .Where(symbol => !string.IsNullOrWhiteSpace(symbol))
                .Select(symbol => symbol.Trim().ToLowerInvariant())
                .Distinct()
                .ToArray();

            if (normalizedSymbols.Length == 0)
            {
                yield break;
            }

            var streams = string.Join(
                "/",
                normalizedSymbols.Select(
                    symbol => $"{symbol}@trade"));

            var url = new Uri(
                $"wss://data-stream.binance.vision/stream?streams={streams}");

            using var socket = new ClientWebSocket();

            await socket.ConnectAsync(
                url,
                cancellationToken);

            var buffer = new byte[8192];

            while (
                socket.State == WebSocketState.Open &&
                !cancellationToken.IsCancellationRequested)
            {
                var json = await ReceiveMessageAsync(
                    socket,
                    buffer,
                    cancellationToken);

                if (json is null)
                {
                    yield break;
                }

                BinanceCombinedTrade? response;

                try
                {
                    response =
                        JsonSerializer.Deserialize<BinanceCombinedTrade>(
                            json);
                }
                catch (JsonException)
                {
                    continue;
                }

                var trade = response?.Data;

                if (trade is null)
                {
                    continue;
                }

                if (!_lastSendTimes.TryGetValue(
                    trade.Symbol,
                    out var lastSendTime))
                {
                    lastSendTime = DateTime.MinValue;
                }

                if (DateTime.UtcNow - lastSendTime >=
                    _interval)
                {
                    _lastSendTimes[trade.Symbol] =
                        DateTime.UtcNow;

                    try
                    { /*
                        double btcprice = double.Parse(_configuration["TargetPrices:btc/usdt"]);
                        double ethprice = double.Parse(_configuration["TargetPrices:eth/usdt"]);
                        double bnbprice = double.Parse(_configuration["TargetPrices:bnb/usdt"]);

                        if (double.Parse(trade.Price) > btcprice && trade.Symbol.Equals("BTCUSDT") 
                            || double.Parse(trade.Price) > ethprice && trade.Symbol.Equals("ETHUSDT") 
                            || double.Parse(trade.Price) > bnbprice && trade.Symbol.Equals("BNBUSDT"))
                        {
                            await _sendMessage.SendMessageAsync(
                            $"{trade.Symbol}: {trade.Price}");
                        } */
                       
                    }
                    catch (Exception exception)
                    {
                        throw new Exception(exception.Message);
                    }
                }

                yield return trade;
            }
        }

        private static async Task<string?> ReceiveMessageAsync(
            ClientWebSocket socket,
            byte[] buffer,
            CancellationToken cancellationToken)
        {
            var message = new StringBuilder();
            WebSocketReceiveResult result;

            do
            {
                result = await socket.ReceiveAsync(
                    new ArraySegment<byte>(buffer),
                    cancellationToken);

                if (result.MessageType ==
                    WebSocketMessageType.Close)
                {
                    return null;
                }

                if (result.MessageType ==
                    WebSocketMessageType.Text)
                {
                    message.Append(
                        Encoding.UTF8.GetString(
                            buffer,
                            0,
                            result.Count));
                }
            }
            while (!result.EndOfMessage);

            return result.MessageType ==
                   WebSocketMessageType.Text
                ? message.ToString()
                : null;
        }
    }
}
