using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wallet.DTOs.Balance
{
    public class TransferResultDto
    {
        public int FromClientId { get; set; }
        public decimal FromNewBalance { get; set; }

        public int ToClientId { get; set; }

        public decimal TransferredAmount { get; set; }
    }
}