using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Cryptography;
using System.Text;
using Wallet.Context;
using Wallet.Model;

namespace Wallet.Middleware
{
    public class AppSeederMiddleware
    {
        private readonly RequestDelegate _next;

        public AppSeederMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            using var scope = context.RequestServices.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            await db.Database.EnsureCreatedAsync();

            if (!db.Clients.Any())
            {
                var clients = new List<ClientsModel>
                {
                    new ClientsModel
                    {
                        Name = "João Silva",
                        Email = "joao@email.com",
                        Password = HashPassword("123456"),
                        Balance = 1000.00m
                    },
                    new ClientsModel
                    {
                        Name = "Maria Souza",
                        Email = "maria@email.com",
                        Password = HashPassword("abcdef"),
                        Balance = 1500.00m
                    }
                };

                db.Clients.AddRange(clients);
                await db.SaveChangesAsync();

                var deposits = new List<TransactionHistoryModel>();
                var transfers = new List<TransactionHistoryModel>();
                var inicio = new DateTime(2025, 12, 1);
                var fim = new DateTime(2026, 1, 23);
                var range = (fim - inicio).Days;

                var amount = 0;

                for (var i = 0; i < 100; i++)
                {
                    amount = Random.Shared.Next(100, 500);
                    var random = Random.Shared;
                    var dataAleatoria = inicio.AddDays(random.Next(range + 1));

                    deposits.Add(new TransactionHistoryModel
                    {
                        ClientId = clients[0].Id,
                        Amount = amount,
                        Type = "deposit",
                        Timestamp = dataAleatoria
                    });

                    clients[0].Balance += amount;
                }

                for (var i = 0; i < 62; i++)
                {
                    amount = Random.Shared.Next(10, 200);
                    var random = Random.Shared;
                    var dataAleatoria = inicio.AddDays(random.Next(range + 1));

                    transfers.Add(new TransactionHistoryModel
                    {
                        ClientId = clients[0].Id,
                        ToClientId = clients[1].Id,
                        Amount = amount,
                        Type = "transfer",
                        Timestamp = dataAleatoria
                    });

                    clients[0].Balance -= amount;
                    clients[1].Balance += amount;

                }

                db.TransactionHistories.AddRange(deposits);
                db.TransactionHistories.AddRange(transfers);
                await db.SaveChangesAsync();
            }



            await _next(context);
        }

        private byte[] HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            return sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        }
    }
}
