using System;

namespace MiBankito.Models
{
    public class TransactionRecord
    {
        public DateTime Timestamp { get; set; }
        public string Platform { get; set; }
        public string Service { get; set; }
        public string Code { get; set; }
        public string Reference { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } // NIO / USD
        public string DocumentType { get; set; } // Cedula / Pasaporte
        public string DocumentNumber { get; set; }
        public string CustomerDocument => $"{DocumentType} {DocumentNumber}";
        public string CashierId { get; set; }
        public string Authorization { get; set; }
        public string RmhTransactionId { get; set; }
        public string Status { get; set; } // Newly added property
    }
}
