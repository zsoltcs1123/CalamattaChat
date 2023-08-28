using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Messaging.Services.RabbitMQ;

public class RabbitMQConsumer : IDisposable
{
    private readonly IModel _channel;
    private readonly string _queueName;

    public RabbitMQConsumer(IConnection connection, string queueName)
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

    public void StartConsuming(EventHandler<BasicDeliverEventArgs> messageHandler)
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += messageHandler;

        _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);
    }

    public void Dispose()
    {
        _channel?.Dispose();
    }
}