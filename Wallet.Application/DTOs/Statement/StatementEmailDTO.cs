using Wallet.Model;

namespace Wallet.Application.DTOs.Statement
{
    public struct StatementEmailDTO
    {
        public string Email { get; set; }
        public List<TransactionHistoryModel> Obj { get; set; }
    }
}
