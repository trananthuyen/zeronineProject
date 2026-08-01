using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zeronineProject.Core.Entities
{
    public class Candle
    {
        public string Symbol { get; set; } = string.Empty;
        public string Interval { get; set; } = string.Empty;

        public DateTime OpenTime { get; set; }
        public DateTime CloseTime { get; set; }

        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }

        public decimal Volume { get; set; }

        // Khối lượng tính theo tài sản quote (USDT)
        public decimal QuoteAssetVolume { get; set; }

        // Số lượng giao dịch
        public int NumberOfTrades { get; set; }

        // Khối lượng mua chủ động (Taker Buy)
        public decimal TakerBuyBaseVolume { get; set; }

        // Giá trị mua chủ động theo quote
        public decimal TakerBuyQuoteVolume { get; set; }

        // Đánh dấu nến đã đóng hay chưa
        public bool IsClosed { get; set; }

    }
}
