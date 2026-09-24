using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zeronineProject.Core.Services
{
    public class TelegramAlertLimiter
    {
        private readonly ConcurrentDictionary<string, DateTime>
        _lastSendTimes = new(StringComparer.OrdinalIgnoreCase);

        private readonly TimeSpan _interval;

        public TelegramAlertLimiter(int intervalMinutes)
        {
            _interval = TimeSpan.FromMinutes(intervalMinutes);
        }

        public bool CanSend(string key)
        {
            DateTime now = DateTime.UtcNow;

            DateTime lastSendTime =
                _lastSendTimes.GetOrAdd(
                    key,
                    DateTime.MinValue);

            return now - lastSendTime >= _interval;
        }

        public DateTime MarkAsSent(string key, string durationMinutes = "0")
        {
            _lastSendTimes[key] = DateTime.UtcNow.AddMinutes(int.Parse(durationMinutes));
            return _lastSendTimes[key];
        }
    }
}
