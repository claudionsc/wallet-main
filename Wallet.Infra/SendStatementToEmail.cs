using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using System.Threading.Tasks;


namespace Wallet.Infra
{
    internal class SendStatementToEmail
    {
        public async Task<bool> Send(dynamic data, IChannel channel, string exchangeName, string routingKeyEmail)
        {
            try
            {
                //var obj = data.Result.Data.Obj;

                var body = JsonSerializer.SerializeToUtf8Bytes(
                            data,
                            new JsonSerializerOptions
                            {
                                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                            }
                        );

                var properties = new BasicProperties
                {
                    Persistent = true,
                    ContentType = "application/json",
                    ContentEncoding = "utf-8"
                };

                await channel.BasicPublishAsync(
                    exchange: exchangeName,
                    routingKey: routingKeyEmail,
                    mandatory: false,
                    basicProperties: properties,
                    body: body
                    );

                return true;
            }
            catch
            {
                return false;
            }

        }
    }
}
