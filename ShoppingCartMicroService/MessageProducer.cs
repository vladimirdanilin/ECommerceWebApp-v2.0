using RabbitMQ.Client;
using System.Text;

namespace ShoppingCartMicroService
{
    public class MessageProducer
    {
        private readonly string _queueName;

        public MessageProducer(string queueName)
        {
            _queueName = queueName;
        }

        public void SendMessage(string message)
        {
            var factory = new ConnectionFactory { HostName = "localhost" };

            using var connection = factory.CreateConnection();

            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: _queueName,
                                durable: false,
                                exclusive: false,
                                autoDelete: false,
                                arguments: null);

            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: "", routingKey: _queueName, basicProperties: null, body: body);
            Console.WriteLine($"[X] sent {message}");
        }
    }
}
