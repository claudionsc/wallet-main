using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wallet.DTOs.Filter
{
    public class TransactionFilterDateDTO
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }  
    }
}