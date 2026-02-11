using Wallet.Model;
using Wallet.DTOs.Client;
using Wallet.Models;

namespace Wallet.Interfaces
{
    public interface IClientsService
    {
        Task<ResponseModel<ClientResponseDTO>> UpdateClient(int id, ClientsDTO updatedClient);
        Task<ResponseModel<ClientsModel>> SearchClient(string email);
        Task<ResponseModel<ClientToken>> CreateClient(ClientsDTO clientsDTO);
        
        Task<ResponseModel<ClientToken>> Login(ClientLoginDTO clientLoginDTO);
        
        Task<ResponseModel<ClientsModel>> DeleteClient(int id);
    }
}
