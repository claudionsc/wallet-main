using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wallet.Application.DTOs.Statement;
using Wallet.Context;
using Wallet.DTOs;
using Wallet.DTOs.Balance;
using Wallet.DTOs.Filter;
using Wallet.Interfaces;
using Wallet.Model;

namespace Wallet.Services
{
    public class BalanceService : IBalanceService
    {
        private readonly AppDbContext _context;

        public BalanceService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ResponseModel<BalanceDto>> AddBalanceAsync(int clientId, decimal amount)
        {
            var response = new ResponseModel<BalanceDto>();

            try
            {
                if (amount <= 0)
                {
                    response.Success = false;
                    response.Message = "O valor adicionado deve ser maior que zero.";
                    return response;
                }

                var client = await _context.Clients.FindAsync(clientId);
                if (client == null)
                {
                    response.Success = false;
                    response.Message = "Cliente não encontrado.";
                    return response;
                }

                client.Balance += amount;

                _context.TransactionHistories.Add(new TransactionHistoryModel
                {
                    ClientId = client.Id,
                    Amount = amount,
                    Type = "add"
                });

                await _context.SaveChangesAsync();

                response.Success = true;
                response.Message = "Saldo adicionado com sucesso.";
                response.Data = new BalanceDto { Balance = client.Balance };
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Erro ao adicionar saldo: {ex.Message}";
            }

            return response;
        }

        public async Task<ResponseModel<TransferResultDto>> TransferBalanceAsync(TransferRequestDto request)
        {
            var response = new ResponseModel<TransferResultDto>();

            try
            {
                if (request.Amount <= 0)
                {
                    response.Success = false;
                    response.Message = "O valor da transferência deve ser maior que zero.";
                    return response;
                }

                if (request.FromClientId == request.ToClientId)
                {
                    response.Success = false;
                    response.Message = "Não é possível transferir para a mesma conta.";
                    return response;
                }

                var fromClient = await _context.Clients.FindAsync(request.FromClientId);
                var toClient = await _context.Clients.FindAsync(request.ToClientId);

                if (fromClient == null || toClient == null)
                {
                    response.Success = false;
                    response.Message = "Cliente de origem ou destino não encontrado.";
                    return response;
                }

                if (fromClient.Balance < request.Amount)
                {
                    response.Success = false;
                    response.Message = "Saldo insuficiente para realizar a transferência.";
                    return response;
                }

                fromClient.Balance -= request.Amount;
                toClient.Balance += request.Amount;

                _context.TransactionHistories.Add(new TransactionHistoryModel
                {
                    ClientId = fromClient.Id,
                    Amount = -request.Amount,
                    Type = "transfer",
                    ToClientId = toClient.Id
                });

                _context.TransactionHistories.Add(new TransactionHistoryModel
                {
                    ClientId = toClient.Id,
                    Amount = request.Amount,
                    Type = "transfer",
                    ToClientId = fromClient.Id
                });

                await _context.SaveChangesAsync();

                response.Success = true;
                response.Message = "Transferência realizada com sucesso.";
                response.Data = new TransferResultDto
                {
                    FromClientId = fromClient.Id,
                    FromNewBalance = fromClient.Balance,
                    ToClientId = toClient.Id,
                    TransferredAmount = request.Amount
                };
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Erro ao realizar transferência: {ex.Message}";
            }

            return response;
        }


        public async Task<ResponseModel<PaginatedResultDto<TransactionHistoryModel>>> GetTransactionHistoryAsync(
            int clientId, int pageNumber, int pageSize, string? type = null, TransactionFilterDateDTO? filter = null)
        {
            var response = new ResponseModel<PaginatedResultDto<TransactionHistoryModel>>();

            try
            {
                var exists = await _context.Clients.AnyAsync(c => c.Id == clientId);
                if (!exists)
                {
                    response.Success = false;
                    response.Message = "Cliente não encontrado.";
                    return response;
                }

                var query = _context.TransactionHistories
                    .Where(t => t.ClientId == clientId || t.ToClientId == clientId)
                    .AsQueryable();

                // Filtro por tipo
                if (!string.IsNullOrEmpty(type))
                {
                    type = type.ToLower();
                    if (type != "add" && type != "transfer")
                    {
                        response.Success = false;
                        response.Message = "Tipo de transação inválido. Use 'add' ou 'transfer'.";
                        return response;
                    }

                    query = query.Where(t => t.Type == type);
                }

                // Filtro por data
                if (filter?.StartDate.HasValue == true)
                {
                    var startUtc = DateTime.SpecifyKind(filter.StartDate.Value, DateTimeKind.Utc);
                    query = query.Where(t => t.Timestamp >= startUtc);
                }

                if (filter?.EndDate.HasValue == true)
                {
                    var endDate = filter.EndDate.Value;

                    // Se a hora for zero (meia-noite), atualiza para a hora atual UTC
                    if (endDate.TimeOfDay == TimeSpan.Zero)
                    {
                        var nowUtc = DateTime.UtcNow;
                        // Usa a data do EndDate, mas substitui hora, minuto, segundo pela hora atual
                        endDate = new DateTime(endDate.Year, endDate.Month, endDate.Day,
                                               nowUtc.Hour, nowUtc.Minute, nowUtc.Second, DateTimeKind.Utc);
                    }
                    else
                    {
                        // Apenas garantir que seja UTC
                        endDate = DateTime.SpecifyKind(endDate, DateTimeKind.Utc);
                    }

                    query = query.Where(t => t.Timestamp <= endDate);
                }
                else
                {
                    // Se EndDate não informado, usa a hora atual UTC
                    var nowUtc = DateTime.UtcNow;
                    query = query.Where(t => t.Timestamp <= nowUtc);
                }


                var totalItems = await query.CountAsync();

                var transactions = await query
                .OrderByDescending(t => t.Timestamp) // Ordena as transações da mais recente para a mais antiga

                // Ignora os registros das páginas anteriores
                // Exemplo: pageNumber = 2, pageSize = 10 => pula os 10 primeiros registros
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize) // Pega apenas a quantidade de registros da página atual
                .ToListAsync();

                response.Success = true;
                response.Message = "Transações encontradas com sucesso.";
                response.Data = new PaginatedResultDto<TransactionHistoryModel>
                {
                    Items = transactions,
                    TotalItems = totalItems,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Erro ao buscar transações: {ex.Message}";
            }

            return response;
        }

        public async Task<ResponseModel<StatementEmailDTO>> GetAllTransactionHistoryAsync(int clientId, string email, string? type = null, TransactionFilterDateDTO? filter = null)
        {
            var response = new ResponseModel<StatementEmailDTO>();

            try
            {
                var exists = await _context.Clients.AnyAsync(c => c.Id == clientId);
                if (!exists)
                {
                    response.Success = false;
                    response.Message = "Cliente não encontrado.";
                    return response;
                }

                var query = _context.TransactionHistories
                    .Where(t => t.ClientId == clientId || t.ToClientId == clientId)
                    .AsQueryable();

                // Filtro por tipo
                if (!string.IsNullOrEmpty(type))
                {
                    type = type.ToLower();
                    if (type != "add" && type != "transfer")
                    {
                        response.Success = false;
                        response.Message = "Tipo de transação inválido. Use 'add' ou 'transfer'.";
                        return response;
                    }

                    query = query.Where(t => t.Type == type);
                }

                // Filtro por data
                if (filter?.StartDate.HasValue != true || filter?.EndDate.HasValue != true)
                {
                    response.Success = false;
                    response.Message = "Data inválida. Favor selecionar uma data de início e uma data de fim";
                    return response;
                }

                var startUtc = DateTime.SpecifyKind(filter.StartDate.Value, DateTimeKind.Utc);
                query = query.Where(t => t.Timestamp >= startUtc);

                var endDate = filter.EndDate.Value;

                // Se a hora for zero (meia-noite), atualiza para a hora atual UTC
                if (endDate.TimeOfDay == TimeSpan.Zero)
                {
                    var nowUTC = DateTime.UtcNow;
                    // Usa a data do EndDate, mas substitui hora, minuto, segundo pela hora atual
                    endDate = new DateTime(endDate.Year, endDate.Month, endDate.Day,
                                           nowUTC.Hour, nowUTC.Minute, nowUTC.Second, DateTimeKind.Utc);
                }
                else
                {
                    // Apenas garantir que seja UTC
                    endDate = DateTime.SpecifyKind(endDate, DateTimeKind.Utc);
                }

                query = query.Where(t => t.Timestamp <= endDate);

                // Se EndDate não informado, usa a hora atual UTC
                var nowUtc = DateTime.UtcNow;
                query = query.Where(t => t.Timestamp <= nowUtc);

                var transactions = query.OrderByDescending(t => t.Timestamp).ToList(); // Ordena as transações da mais recente para a mais antiga

                response.Success = true;
                response.Message = "Transações encontradas com sucesso.";
                response.Data = new StatementEmailDTO
                {
                    Email = email,
                    Obj = transactions
                };
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Erro ao buscar transações: {ex.Message}";
            }

            return response;
        }
    }
}
