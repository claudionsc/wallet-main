using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wallet.Application.DTOs.Statement;
using Wallet.Model;

namespace Wallet.Interfaces
{
    public interface IStatementService
    {
        Task CallPoducer(Task<ResponseModel<StatementEmailDTO>> transactionHistory);
        
    }
}