using System.Text;
using RabbitMQ.Client;

namespace Messaging.Services.RabbitMQ;

public class RabbitMQPublisher : IDisposable
{
    private readonly IModel _channel;
    private readonly string _queueName;

    public RabbitMQPublisher(IConnection connection, string queueName)
    {
        _channel = connection.CreateModel();
        _queueName = queueName;
        _channel.QueueDeclare(
            queue: _queueName, 
            durable: false, 
            exclusive: false, 
            autoDelete: false, 
            arguments: null);
    }

    public void PublishMessage(string message)
    {
        var body = Encoding.UTF8.GetBytes(message);
        _channel.BasicPublish(
            exchange: "", 
            routingKey: _queueName, 
            basicProperties: null, 
            body: body);
    }

    public void Dispose()
    {
        _channel?.Dispose();
    }
}