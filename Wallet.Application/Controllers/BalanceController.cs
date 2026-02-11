using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wallet.DTOs;
using Wallet.DTOs.Balance;
using Wallet.DTOs.Filter;
using Wallet.Interfaces;
using Wallet.Model;

namespace Wallet.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BalanceController : ControllerBase
    {
        private readonly IBalanceService _balanceService;

        public BalanceController(IBalanceService balanceService)
        {
            _balanceService = balanceService;
        }

        [Authorize]
        [HttpPost("add/{clientId}")]
        public async Task<ActionResult<ResponseModel<BalanceDto>>> AddBalance(int clientId, [FromBody] decimal amount)
        {
            var result = await _balanceService.AddBalanceAsync(clientId, amount);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [Authorize]
        [HttpPost("transfer")]
        public async Task<ActionResult<ResponseModel<TransactionHistoryModel>>> TransferBalance([FromBody] TransferRequestDto request)
        {
            var result = await _balanceService.TransferBalanceAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }


        [Authorize]
        [HttpGet("get")]
        public async Task<ActionResult<ResponseModel<PaginatedResultDto<TransferRequestDto>>>> GetTransactions(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] TransactionFilterDateDTO? filter = null,
            [FromQuery] string? type = null)
        {
            var clientId = int.Parse(User.FindFirst("id")!.Value);
            var result = await _balanceService.GetTransactionHistoryAsync(clientId, pageNumber, pageSize, type, filter);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        
        [Authorize]
        [HttpGet("post")]
        public async Task<ActionResult> GetStatement([FromQuery] DateTime inicial, [FromQuery] DateTime final, [FromQuery] string email)
        {
            
            return Ok();
        }
    }
}
