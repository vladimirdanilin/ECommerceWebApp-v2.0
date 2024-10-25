using Microsoft.EntityFrameworkCore.Metadata;
using ProductMicroService.Data.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Channels;

namespace ProductMicroService
{
    public class ConsumerService : BackgroundService
    {
        private RabbitMQ.Client.IModel _channel;
        private IConnection _connection;
        private readonly IServiceProvider _serviceProvider;

        public ConsumerService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var factory = new ConnectionFactory { HostName = "localhost" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            ProcessQueue("decrease_product_quantity_queue", async (productId, quantity) =>
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var inventoryService = scope.ServiceProvider.GetRequiredService<IInventoryService>();
                    await inventoryService.DecreaseProductQuantityAsync(productId, quantity);
                }
            });

            ProcessQueue("increase_product_quantity_queue", async (productId, quantity) =>
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var inventoryService = scope.ServiceProvider.GetRequiredService<IInventoryService>();
                    await inventoryService.IncreaseProductQuantityAsync(productId, quantity);
                }
            });

            return Task.CompletedTask;
        }

        private void ProcessQueue(string queueName, Func<int, int, Task> processMessage)
        {
            _channel.QueueDeclare(queue: queueName,
                                durable: false,
                                exclusive: false,
                                autoDelete: false,
                                arguments: null);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var productQuantityMessage = JsonSerializer.Deserialize<ProductQuantityMessage>(message);

                await processMessage(productQuantityMessage.ProductId, productQuantityMessage.Quantity);
            };

            _channel.BasicConsume(queue: queueName,
                    autoAck: true,
                    consumer: consumer);
        }

        public class ProductQuantityMessage
        {
            public int ProductId { get; set; }

            public int Quantity { get; set; }
        }

        public override void Dispose()
        { 
            _channel?.Close();
            _channel?.Dispose();
            base.Dispose();
        }
    }
}
