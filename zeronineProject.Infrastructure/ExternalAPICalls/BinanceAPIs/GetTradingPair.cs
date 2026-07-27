using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using zeronineProject.Core.Entities;

namespace zeronineProject.Infrastructure.ExternalAPICalls.BinanceAPIs
{
    public class GetTradingPair
    {
        public async Task<TradingPair?> GetTradingPairAsync(string symbol, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(symbol))
            {
                throw new ArgumentException(
                    "Symbol không được để trống.",
                    nameof(symbol)
                );
            }

            symbol = symbol
                .Replace("/", string.Empty)
                .Trim()
                .ToLowerInvariant();

            using var socket = new ClientWebSocket();

            var url = new Uri(
                $"wss://data-stream.binance.vision/ws/{symbol}@trade"
            );

            await socket.ConnectAsync(url, cancellationToken);

            var buffer = new byte[8192];
            var message = new StringBuilder();

            while (
                socket.State == WebSocketState.Open &&
                !cancellationToken.IsCancellationRequested
            )
            {
                WebSocketReceiveResult result;

                do
                {
                    result = await socket.ReceiveAsync(
                        new ArraySegment<byte>(buffer),
                        cancellationToken
                    );

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await CloseSocketAsync(
                            socket,
                            cancellationToken
                        );

                        return null;
                    }

                    message.Append(
                        Encoding.UTF8.GetString(
                            buffer,
                            0,
                            result.Count
                        )
                    );
                }
                while (!result.EndOfMessage);

                if (result.MessageType != WebSocketMessageType.Text)
                {
                    message.Clear();
                    continue;
                }

                var tradingPair =
                    JsonSerializer.Deserialize<TradingPair>(
                        message.ToString()
                    );

                await CloseSocketAsync(
                    socket,
                    cancellationToken
                );

                return tradingPair;
            }

            return null;
        }

        private static async Task CloseSocketAsync(
            ClientWebSocket socket,
            CancellationToken cancellationToken)
        {
            if (
                socket.State == WebSocketState.Open ||
                socket.State == WebSocketState.CloseReceived
            )
            {
                await socket.CloseAsync(
                    WebSocketCloseStatus.NormalClosure,
                    "Closing",
                    cancellationToken
                );
            }
        }
    }
}
