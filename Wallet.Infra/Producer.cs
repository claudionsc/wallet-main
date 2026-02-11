using Microsoft.VisualBasic;
using RabbitMQ.Client;
using System.Text.Json;



namespace Wallet.Infra
{
    public class Producer
    {

        const string exchangeName = "statment.exchange";
        const string queuNameEmail = "statement.email";
        const string queueNamePDF = "statement.PDF";

        const string routingKeyEmail = "routingKey.email";
        const string routingKeyPDF = "routingKey.PDF";

        // int port = int.Parse(Environment.GetEnvironmentVariable("PORT"));


        public async Task producer(dynamic data)
        {
            DotNetEnv.Env.Load();
            

            if (!data.Result.Success) await Task.FromException(new Exception());

            var HostName = Environment.GetEnvironmentVariable("HOSTNAME");

            try
            {

                var factory = new RabbitMQ.Client.ConnectionFactory()
                {
                    HostName = Environment.GetEnvironmentVariable("HOSTNAME"),
                    Port = int.Parse(Environment.GetEnvironmentVariable("PORT")),
                    UserName = Environment.GetEnvironmentVariable("USERNAME"),
                    Password = Environment.GetEnvironmentVariable("PASSWORD"),
                    VirtualHost = Environment.GetEnvironmentVariable("VIRTUALHOST"),
                    AutomaticRecoveryEnabled = true,
                    NetworkRecoveryInterval = TimeSpan.FromSeconds(10),
                };

                await using var connection = await factory.CreateConnectionAsync();
                await using var channel = await connection.CreateChannelAsync();

                await channel.ExchangeDeclareAsync(
            exchange: exchangeName,
            type: RabbitMQ.Client.ExchangeType.Direct,
            durable: true,
            autoDelete: false
        );

                await channel.QueueDeclareAsync(
                    queue: queuNameEmail,
                    durable: true,
                    exclusive: false,
                    autoDelete: false
                 );

                await channel.QueueDeclareAsync(
                    queue: queueNamePDF,
                    durable: true,
                    exclusive: false,
                    autoDelete: false
                 );

                await channel.QueueBindAsync(
                    queue: queuNameEmail,
                    exchange: exchangeName,
                    routingKey: routingKeyEmail
                );

                await channel.QueueBindAsync(
                    queue: queuNameEmail,
                    exchange: exchangeName,
                    routingKey: routingKeyPDF
                );

                // enviar

                SendStatementToEmail send = new SendStatementToEmail();

                send.Send(data, channel, exchangeName, routingKeyEmail);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
