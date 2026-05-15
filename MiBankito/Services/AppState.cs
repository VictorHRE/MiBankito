using System;
using System.Collections.Generic;
using System.Linq;
using MiBankito.Models;

namespace MiBankito.Services
{
    public static class AppState
    {
        private static readonly List<TransactionRecord> _txns = new List<TransactionRecord>();
        public static IReadOnlyList<TransactionRecord> Transactions => _txns;

        public static event EventHandler Changed;

        public static void Add(TransactionRecord r)
        {
            if (r == null) return;
            r.Timestamp = r.Timestamp == default ? DateTime.Now : r.Timestamp;
            _txns.Insert(0, r);
            Changed?.Invoke(null, EventArgs.Empty);
        }

        public static void NotifyChanged()
        {
            Changed?.Invoke(null, EventArgs.Empty);
        }

        public static IEnumerable<TransactionRecord> Filter(string platform, string refText, DateTime? from, DateTime? to)
        {
            var q = _txns.AsEnumerable();
            if (!string.IsNullOrWhiteSpace(platform) && platform != "*")
                q = q.Where(x => string.Equals(x.Platform, platform, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrWhiteSpace(refText))
                q = q.Where(x => (x.Reference ?? string.Empty).IndexOf(refText, StringComparison.OrdinalIgnoreCase) >= 0);
            if (from.HasValue) q = q.Where(x => x.Timestamp >= from.Value);
            if (to.HasValue) q = q.Where(x => x.Timestamp <= to.Value);
            return q;
        }

        public static int TransactionsTodayCount()
        {
            var today = DateTime.Today;
            return _txns.Count(x => x.Timestamp.Date == today);
        }

        public static decimal TotalAmountToday()
        {
            var today = DateTime.Today;
            return _txns.Where(x => x.Timestamp.Date == today).Sum(x => x.Amount);
        }
    }
}
