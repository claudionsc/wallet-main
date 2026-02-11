using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wallet.Application.DTOs.Statement;
using Wallet.Infra;
using Wallet.Interfaces;
using Wallet.Model;



namespace Wallet.Services.Statement
{
    public class StatementService : IStatementService
    {
        private readonly Producer _producer;
        public StatementService(Producer producer)
        {
            _producer = producer;
        }
        public async Task CallPoducer(Task<ResponseModel<StatementEmailDTO>> transactionHistory)
        {
             await _producer.producer(transactionHistory);

        }
    }
}