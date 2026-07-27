using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace zeronineProject.Infrastructure.ExternalAPICalls.TelegramAPIs
{
    public class SendMessage
    {
        private readonly HttpClient _httpClient;
        private readonly TelegramOptions _options;

        public SendMessage(HttpClient httpClient,
                           IOptions<TelegramOptions> options)
        {
            _httpClient = httpClient;
            _options = options.Value;
        }

        public async Task SendMessageAsync(string message)
        {
            var url =
                $"https://api.telegram.org/bot{_options.BotToken}/sendMessage";

            var body = new
            {
                chat_id = _options.ChatId,
                text = message
            };

            await _httpClient.PostAsJsonAsync(url, body);
        }
    }
}
