using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zeronineProject.Core.Services
{
    public class RSICheckServices
    {
        

        public RSICheckServices()
        {

        }

        public string RSICheck(string symbol, string interval, double preveRSI, double RSI)
        {
            if(preveRSI > 70 && RSI > 70)
            {
                return $"{symbol} ({interval}) already has above 70: ";
            }
            else if(preveRSI < 30 && RSI < 30)
            {
                return $"{symbol} ({interval}) already has below 30: ";
            }
            else if(preveRSI > 70 && Math.Round(RSI, 0) <= 70 || preveRSI < 30 && RSI >= 30)
            {
                return $"{symbol} ({interval}) has position target entry";
            }
            

            return "none";
        }
    }
}
