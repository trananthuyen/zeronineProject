using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace zeronineProject.Core.Entities
{
    public class TradingPair
    {
        [JsonPropertyName("s")]
        public string Symbol { get; set; } 

        [JsonPropertyName("p")]
        public string Price { get; set; } 

        [JsonPropertyName("q")]
        public string Quantity { get; set; } 

        [JsonPropertyName("T")]
        public long TradeTime { get; set; }
    }
}
