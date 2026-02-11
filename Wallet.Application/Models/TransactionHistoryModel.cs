using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wallet.Model
{
    public class TransactionHistoryModel
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; } = string.Empty;
        public int? ToClientId { get; set; } 
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}