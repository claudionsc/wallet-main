
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wallet.DTOs.Client;
using Wallet.Interfaces;
using Wallet.Model;

namespace Wallet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly IClientsService _clients;
        public ClientsController(IClientsService clients)
        {
            _clients = clients;
        }

        [HttpGet("get")]
        public async Task<ActionResult<ResponseModel<ClientsModel>>> SearchUser(string email)
        {
            var user = await _clients.SearchClient(email);
            return Ok(user);    
        }

        [HttpPost("create")]
        public async Task<ActionResult<ResponseModel<ClientsModel>>> CreateUser(ClientsDTO clientsDTO)
        {
            var user = await _clients.CreateClient(clientsDTO);
            return Ok(user);
        } 

        [Authorize]
        [HttpPut("update/{id}")]
        public async Task<ActionResult<ResponseModel<ClientsDTO>>> UpdateUser(int id, ClientsDTO clientsObj)
        {
            var user = await _clients.UpdateClient(id, clientsObj);
            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<ActionResult<ResponseModel<ClientLoginDTO>>> Login(ClientLoginDTO clientLoginDTO)
        {
            var login = await _clients.Login(clientLoginDTO);
            return Ok(login);
        }

        [Authorize]
        [HttpDelete("delete/{id}")]
        public async Task<ActionResult<ResponseModel<ClientsModel>>> Delete(int id)
        {
            var delete = await _clients.DeleteClient(id);
            return Ok(delete);
        }

    }
}
