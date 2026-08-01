using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace zeronineProject.Core.Entities
{
    public class BinanceCombinedTrade
    {
        [JsonPropertyName("stream")]
        public string Stream { get; set; } = string.Empty;

        [JsonPropertyName("data")]
        public TradingPair Data { get; set; } = new();
    }
}
