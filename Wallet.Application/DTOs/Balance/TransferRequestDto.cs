using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wallet.DTOs.Balance
{
    public class TransferRequestDto
    {
        public int FromClientId { get; set; }
        public int ToClientId { get; set; }
        public decimal Amount { get; set; }
    }
}