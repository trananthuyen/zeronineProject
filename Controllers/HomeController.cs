using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.WebSockets;
using System.Threading;
using zeronineProject.Core.Entities;
using zeronineProject.Core.Services;
using zeronineProject.Infrastructure.ExternalAPICalls.BinanceAPIs;
using zeronineProject.Infrastructure.ExternalAPICalls.TelegramAPIs;

namespace zeronineProject.UI.Controllers
{

    public class HomeController : Controller
    {
        public readonly GetCandles _getCandles;
        public readonly RSIAnalysisServices _rsiAnalysisService;
        public readonly RSICheckServices _rsiCheckServices;

        public readonly SendMessage _sendMessage;

        private readonly TelegramAlertLimiter _alertLimiter;

        public HomeController(GetCandles getCandles, RSIAnalysisServices rsiAnalysisService, SendMessage sendMessage, RSICheckServices rsiCheckServices, TelegramAlertLimiter telegramAlertLimiter)
        {
            _getCandles = getCandles;
            _rsiAnalysisService = rsiAnalysisService;
            _sendMessage = sendMessage;
            _rsiCheckServices = rsiCheckServices;
            _alertLimiter = telegramAlertLimiter;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Analysis([FromQuery] string symbol, [FromQuery] string interval)
        {
            List<Candle> listCandle = await _getCandles.GetCandlesAsync(symbol, interval, 200);

            var result = Math.Round(_rsiAnalysisService.GetRSIAnalysis(listCandle), 2);

            var previousResult = Math.Round(_rsiAnalysisService.GetRSIPreveAnalysis(listCandle), 2);

            string alertType =
        _rsiCheckServices.RSICheck(
            symbol,
            interval,
            previousResult,
            result);

            if (alertType != "none" &&
                _alertLimiter.CanSend(alertType))
            {
                try
                {
                    await _sendMessage.SendMessageAsync(
                        $"{alertType} current RSI {result}");

                    _alertLimiter.MarkAsSent(alertType);
                }
                catch (Exception exception)
                {
                    return StatusCode(
                        StatusCodes.Status502BadGateway,
                        $"Không thể gửi Telegram: {exception.Message}");
                }
            }


            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Mutersinotification([FromQuery] string symbol, [FromQuery] string interval, [FromQuery] string durationMinutes)
        {
            List<Candle> listCandle = await _getCandles.GetCandlesAsync(symbol, interval, 200);

            var result = Math.Round(_rsiAnalysisService.GetRSIAnalysis(listCandle), 2);

            var previousResult = Math.Round(_rsiAnalysisService.GetRSIPreveAnalysis(listCandle), 2);

            string alertType =
        _rsiCheckServices.RSICheck(
            symbol,
            interval,
            previousResult,
            result);

            
                try
                {
                    _alertLimiter.MarkAsSent(alertType, durationMinutes);
                }
                catch (Exception exception)
                {
                    return StatusCode(
                        StatusCodes.Status502BadGateway,
                        $"Không thể gửi Telegram: {exception.Message}");
                }
            


            return Ok(result);
            
        }
    }
}
