using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Vibe.Test.Servcie.Interfaces;

public interface IMessageService
{
    Task<bool> PublishMessageAsync(string queueName, string message);
    Task<string?> ConsumeMessageAsync(string queueName);
}
