using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Wallet.DTOs.Filter;
using Wallet.Interfaces;
using Wallet.Model;
using Wallet.Services.Statement;

namespace Wallet.Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatementController : Controller
    {
        private readonly IStatementService _statementService;
        private readonly IBalanceService _balanceService;

        public StatementController(IStatementService statementService, IBalanceService balanceService)
        {
            _statementService = statementService;
            _balanceService = balanceService;
        }

        [HttpPost("get")]
        public IActionResult Generate([FromQuery] string email, [FromQuery] string? type = null, [FromQuery] TransactionFilterDateDTO? filter = null)
        {
            //var clientId = int.Parse(User.FindFirst("id")!.Value);
            var clientId = 1;

            var data = _balanceService.GetAllTransactionHistoryAsync(clientId, email, type, filter);

           var statement = _statementService.CallPoducer(data);

           return Ok();
        }
    }
}