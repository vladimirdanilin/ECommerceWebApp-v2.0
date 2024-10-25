using RabbitMQ.Client;
using System.Text;

namespace OrderMicroService
{
    public class MessageProducer
    {
        public void SendMessage(string message, string queueName)
        {
            var factory = new ConnectionFactory { HostName = "localhost" };

            using var connection = factory.CreateConnection();

            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: queueName,
                                durable: false,
                                exclusive: false,
                                autoDelete: false,
                                arguments: null);

            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: body);
            Console.WriteLine($"[X] sent {message}");
        }
    }
}
