using Microsoft.AspNetCore.SignalR;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using zeronineProject.Core.Entities;
using zeronineProject.Infrastructure.ExternalAPICalls.TelegramAPIs;
using zeronineProject.UI.Hubs;

namespace zeronineProject.UI.HostedServices
{
    public class BinancePriceBackgroundService : BackgroundService
    {
        private readonly IHubContext<CryptoHub> _hubContext;
        private readonly SendMessage _sendMessage;
        private DateTime _lastSendTime = DateTime.MinValue;
        private readonly TimeSpan _interval = TimeSpan.FromSeconds(30);

        public BinancePriceBackgroundService(
            IHubContext<CryptoHub> hubContext, SendMessage sendMessage)
        {
            _hubContext = hubContext;
            _sendMessage = sendMessage;
        }

        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ReceivePricesAsync(stoppingToken);
                }
                catch (OperationCanceledException)
                    when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.Message);

                    await Task.Delay(
                        TimeSpan.FromSeconds(5),
                        stoppingToken
                    );
                }
            }
        }

        private async Task ReceivePricesAsync(
            CancellationToken cancellationToken)
        {
            using var socket = new ClientWebSocket();

            var url = new Uri(
                "wss://data-stream.binance.vision/ws/ethusdt@trade"
            );

            await socket.ConnectAsync(url, cancellationToken);

            var buffer = new byte[8192];

            while (
                socket.State == WebSocketState.Open &&
                !cancellationToken.IsCancellationRequested
            )
            {
                var message = new StringBuilder();
                WebSocketReceiveResult result;

                do
                {
                    result = await socket.ReceiveAsync(
                        new ArraySegment<byte>(buffer),
                        cancellationToken
                    );

                    if (result.MessageType ==
                        WebSocketMessageType.Close)
                    {
                        return;
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

                if (result.MessageType !=
                    WebSocketMessageType.Text)
                {
                    continue;
                }

                var tradingPair =
                    JsonSerializer.Deserialize<TradingPair>(
                        message.ToString()
                    );

                if (tradingPair is null)
                {
                    continue;
                }

                if (DateTime.UtcNow - _lastSendTime >= _interval)
                {
                    _lastSendTime = DateTime.UtcNow;

                    await _sendMessage.SendMessageAsync(
                        $"ETH: {tradingPair.Price}"
                    );
                }



                await _hubContext.Clients.All.SendAsync(
                    "ReceivePrice",
                    tradingPair,
                    cancellationToken
                );
            }
        }
    }
}
