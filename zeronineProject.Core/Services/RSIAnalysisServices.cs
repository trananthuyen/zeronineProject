using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using zeronineProject.Core.Entities;

namespace zeronineProject.Core.Services
{
    public class RSIAnalysisServices
    {
        //Calculate Current RSI
        public double GetRSIAnalysis(
    IReadOnlyList<Candle> candles,
    int period = 14)
        {
            ArgumentNullException.ThrowIfNull(candles);

            if (period <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(period),
                    "Period phải lớn hơn 0.");
            }

            if (candles.Count < period + 1)
            {
                throw new InvalidOperationException(
                    $"Cần ít nhất {period + 1} cây nến để tính RSI {period}.");
            }

            List<Candle> orderedCandles = candles
                .OrderBy(candle => candle.OpenTime)
                .ToList();

            List<double> changes =
                CalculateChanges(orderedCandles);

            var (gains, losses) =
                CalculateGainLoss(changes);

            // Trung bình đơn dùng để khởi tạo RSI đầu tiên
            double averageGain =
                gains.Take(period).Average();

            double averageLoss =
                losses.Take(period).Average();

            // Wilder smoothing cho các nến tiếp theo
            for (int i = period; i < changes.Count; i++)
            {
                averageGain =
                    (
                        averageGain * (period - 1) +
                        gains[i]
                    ) / period;

                averageLoss =
                    (
                        averageLoss * (period - 1) +
                        losses[i]
                    ) / period;
            }

            return CalculateRSI(
                averageGain,
                averageLoss);
        }

        //Calculate History preve RSI
        public double GetRSIPreveAnalysis(
    IReadOnlyList<Candle> candles,
    int period = 14)
        {
            ArgumentNullException.ThrowIfNull(candles);

            if (period <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(period),
                    "Period phải lớn hơn 0.");
            }

            if (candles.Count < period + 1)
            {
                throw new InvalidOperationException(
                    $"Cần ít nhất {period + 1} cây nến để tính RSI {period}.");
            }

            List<Candle> orderedCandles = candles
                .OrderBy(candle => candle.OpenTime)
                .ToList();

            orderedCandles.Remove(orderedCandles.Last()); //remove the candle has'nt closed yet

            List<double> changes =
                CalculateChanges(orderedCandles);

            var (gains, losses) =
                CalculateGainLoss(changes);

            // Trung bình đơn dùng để khởi tạo RSI đầu tiên
            double averageGain =
                gains.Take(period).Average();

            double averageLoss =
                losses.Take(period).Average();

            // Wilder smoothing cho các nến tiếp theo
            for (int i = period; i < changes.Count; i++)
            {
                averageGain =
                    (
                        averageGain * (period - 1) +
                        gains[i]
                    ) / period;

                averageLoss =
                    (
                        averageLoss * (period - 1) +
                        losses[i]
                    ) / period;
            }

            return CalculateRSI(
                averageGain,
                averageLoss);
        }

        private static List<double> CalculateChanges(
            IReadOnlyList<Candle> candles)
        {
            List<double> result = new();

            for (int i = 1; i < candles.Count; i++)
            {
                double change =
                    (double)(
                        candles[i].Close -
                        candles[i - 1].Close
                    );

                result.Add(change);
            }

            return result;
        }

        private static (
            List<double> Gains,
            List<double> Losses)
            CalculateGainLoss(
                IEnumerable<double> changes)
        {
            List<double> gains = new();
            List<double> losses = new();

            foreach (double change in changes)
            {
                gains.Add(Math.Max(change, 0));
                losses.Add(Math.Max(-change, 0));
            }

            return (gains, losses);
        }

        private static double CalculateRSI(
            double averageGain,
            double averageLoss)
        {
            if (averageGain == 0 &&
                averageLoss == 0)
            {
                return 50;
            }

            if (averageLoss == 0)
            {
                return 100;
            }

            if (averageGain == 0)
            {
                return 0;
            }

            double rs =
                averageGain / averageLoss;

            return 
                100 - 100 / (1 + rs);
        }
    }

}
