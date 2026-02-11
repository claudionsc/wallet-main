using System.Threading.Tasks;
using Wallet.Application.DTOs.Statement;
using Wallet.DTOs;
using Wallet.DTOs.Balance;
using Wallet.DTOs.Filter;
using Wallet.Model;

namespace Wallet.Interfaces
{
    public interface IBalanceService
    {
        Task<ResponseModel<BalanceDto>> AddBalanceAsync(int clientId, decimal amount);
        Task<ResponseModel<TransferResultDto>> TransferBalanceAsync(TransferRequestDto transferRequest);
        Task<ResponseModel<PaginatedResultDto<TransactionHistoryModel>>> GetTransactionHistoryAsync(int clientId, int pageNumber, int pageSize, string? type = null, TransactionFilterDateDTO? filter = null);
        Task<ResponseModel<StatementEmailDTO>> GetAllTransactionHistoryAsync(int clientId, string email, string? type = null, TransactionFilterDateDTO? filter = null);

    }
}
