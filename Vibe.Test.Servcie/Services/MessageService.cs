using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Vibe.Test.Servcie.Interfaces;

namespace Vibe.Test.Servcie.Services;

public class MessageService : IMessageService
{
    private readonly IConnection _connection;

    public MessageService(IConnection connection)
    {
        _connection = connection;
    }

    public async Task<bool> PublishMessageAsync(string queueName, string message)
    {
        try
        {
            using var channel = _connection.CreateModel();
            
            channel.QueueDeclare(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var body = Encoding.UTF8.GetBytes(message);
            
            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            channel.BasicPublish(
                exchange: "",
                routingKey: queueName,
                basicProperties: properties,
                body: body);

            return await Task.FromResult(true);
        }
        catch
        {
            return false;
        }
    }

    public async Task<string?> ConsumeMessageAsync(string queueName)
    {
        try
        {
            using var channel = _connection.CreateModel();
            
            channel.QueueDeclare(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var result = channel.BasicGet(queueName, autoAck: true);
            
            if (result == null)
                return null;

            var message = Encoding.UTF8.GetString(result.Body.ToArray());
            return await Task.FromResult(message);
        }
        catch
        {
            return null;
        }
    }
}
