using Microsoft.EntityFrameworkCore;
using Wallet.Context;
using Wallet.DTOs.Client;
using Wallet.Interfaces;
using Wallet.Model;
using Wallet.Models;
using Wallet.Services.Auth;
using Wallet.Utils;

namespace Wallet.Services.Clients
{
    public class ClientsService : IClientsService
    {
        private readonly AppDbContext _context;
        private readonly JwtService _jwt;
        public ClientsService(AppDbContext context, JwtService jwt)
        {
            _context = context;
            _jwt = jwt;
        }
        public async Task<ResponseModel<ClientResponseDTO>> UpdateClient(int id, ClientsDTO updatedUser)
        {
            var resposta = new ResponseModel<ClientResponseDTO>();

            try
            {
                var client = await _context.Clients.FirstOrDefaultAsync(u => u.Id == id);
                if (client == null)
                {
                    resposta.Message = "Usuário não encontrado.";
                    resposta.Success = false;
                    return resposta;
                }

                client.Name = updatedUser.Name;
                client.Email = updatedUser.Email;

                if (!string.IsNullOrWhiteSpace(updatedUser.Password))
                {
                    client.Password = HashHelper.ComputeSha256Hash(updatedUser.Password);
                }

                _context.Clients.Update(client);
                await _context.SaveChangesAsync();

                resposta.Data = new ClientResponseDTO
                {
                    Id = client.Id,
                    Name = client.Name,
                    Email = client.Email,
                };
                resposta.Message = "Usuário atualizado com sucesso.";
                resposta.Success = true;
            }
            catch (Exception ex)
            {
                resposta.Message = ex.Message;
                resposta.Success = false;
            }

            return resposta;
        }

        public async Task<ResponseModel<ClientsModel>> SearchClient(string email)
        {
            var resposta = new ResponseModel<ClientsModel>();
            try
            {
                var client = await _context.Clients.FirstOrDefaultAsync(x => x.Email == email);

                if (client == null)
                {
                    resposta.Message = "Nenhum registro localizado";
                    resposta.Success = false;
                    return resposta;
                }

                resposta.Data = client;
                resposta.Message = "Usuário exibido com sucesso";
                resposta.Success = true;
            }
            catch (Exception e)
            {
                resposta.Message = e.Message;
                resposta.Success = false;
            }

            return resposta;
        }

        public async Task<ResponseModel<ClientToken>> CreateClient(ClientsDTO clientsDto)
        {
            var resposta = new ResponseModel<ClientToken>();

            try
            {
                var client = await _context.Clients.FirstOrDefaultAsync(x => x.Email == clientsDto.Email);

                if (client != null)
                {
                    resposta.Success = false;
                    resposta.Message = "Usuário já cadastrado";
                    return resposta;
                }
                var clientEntity = new ClientsModel()
                {
                    Name = clientsDto.Name,
                    Email = clientsDto.Email,
                    Password = HashHelper.ComputeSha256Hash(clientsDto.Password),
                };

                _context.Clients.Add(clientEntity);
                await _context.SaveChangesAsync();

                var token = _jwt.GenerateToken(clientEntity);

                resposta.Data = new ClientToken { Token = token };
                resposta.Message = "Usuário criado com sucesso";
                resposta.Success = true;
            }
            catch (Exception e)
            {
                resposta.Message = e.Message;
                resposta.Success = false;
            }

            return resposta;
        }

        public async Task<ResponseModel<ClientToken>> Login(ClientLoginDTO clientLoginDTO)
        {
            var resposta = new ResponseModel<ClientToken>();

            try
            {
                var client = await _context.Clients.FirstOrDefaultAsync(x => x.Email == clientLoginDTO.Email);

                if (client == null)
                {
                    resposta.Success = false;
                    resposta.Message = "Usuário não existe";
                    return resposta;
                }

                var inputPasswordHash = HashHelper.ComputeSha256Hash(clientLoginDTO.Password);
                if (!client.Password.SequenceEqual(inputPasswordHash))
                {
                    resposta.Success = false;
                    resposta.Message = "Senha incorreta";
                    return resposta;
                }

                var token = _jwt.GenerateToken(client);

                resposta.Data = new ClientToken { Token = token };
                resposta.Message = "Login realizado com sucesso";
                resposta.Success = true;
            }
            catch (Exception e)
            {
                resposta.Message = e.Message;
                resposta.Success = false;
            }

            return resposta;
        }

        public async Task<ResponseModel<ClientsModel>> DeleteClient(int id)
        {
            var resposta = new ResponseModel<ClientsModel>();

            try
            {
                var client = await _context.Clients.FirstOrDefaultAsync(x => x.Id == id);

                if (client == null)
                {
                    resposta.Message = "Usuário não encontrado";
                    resposta.Success = false;
                    return resposta;
                }

                _context.Clients.Remove(client);
                await _context.SaveChangesAsync();

                resposta.Message = "Usuário excluído com sucesso";
                resposta.Success = true;
            }
            catch (Exception e)
            {
                resposta.Message = e.Message;
                resposta.Success = false;
            }

            return resposta;
        }
    }
}
