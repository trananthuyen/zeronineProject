using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zeronineProject.Infrastructure.ExternalAPICalls.TelegramAPIs
{
    public class TelegramOptions
    {
        public string BotToken { get; set; } = "";
        public string ChatId { get; set; } = "";
    }
}
