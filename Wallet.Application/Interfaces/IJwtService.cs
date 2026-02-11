using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wallet.Model;

namespace Wallet.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(ClientsModel client);
    }
}