using Microsoft.AspNetCore.SignalR;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using zeronineProject.Core.Entities;
using zeronineProject.Infrastructure.ExternalAPICalls.BinanceAPIs;
using zeronineProject.Infrastructure.ExternalAPICalls.TelegramAPIs;
using zeronineProject.UI.Hubs;

namespace zeronineProject.UI.HostedServices
{
    public class BinancePriceBackgroundService : BackgroundService
    {
        private readonly IHubContext<CryptoHub> _hubContext;
        private readonly GetStreamPrices _getStreamPrices;
        

        private readonly string[] _symbols =
        {
        "btcusdt",
        "ethusdt",
        "bnbusdt"
        };

        public BinancePriceBackgroundService(
            IHubContext<CryptoHub> hubContext, GetStreamPrices getStreamPrices)
        {
            _hubContext = hubContext;
            _getStreamPrices = getStreamPrices;
        }

        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            await foreach (
            var trade in _getStreamPrices.StreamPricesAsync(
                _symbols,
                stoppingToken))
            {
                await _hubContext.Clients.All.SendAsync(
                    "ReceivePrice",
                    trade,
                    stoppingToken);
            }
        }

       
    }
}
