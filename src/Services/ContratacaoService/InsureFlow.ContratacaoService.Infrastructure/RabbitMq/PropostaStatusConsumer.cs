using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace InsureFlow.ContratacaoService.Infrastructure.RabbitMq
{
    public class PropostaStatusConsumer : BackgroundService
    {
        private readonly string _hostName;
        private IConnection? _connection;
        private IModel? _channel;
        private readonly IServiceScopeFactory _scopeFactory;

        public PropostaStatusConsumer(string hostName, IServiceScopeFactory scopeFactory)
        {
            _hostName = hostName;
            _scopeFactory = scopeFactory;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            var factory = new ConnectionFactory() { HostName = _hostName };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare("proposta.status", ExchangeType.Fanout, durable: true);
            var queueName = _channel.QueueDeclare().QueueName;
            _channel.QueueBind(queue: queueName, exchange: "proposta.status", routingKey: string.Empty);

            var consumer = new EventingBasicConsumer(_channel!);
            consumer.Received += (sender, ea) =>
            {
                var body = ea.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);
                try
                {
                    var doc = JsonSerializer.Deserialize<JsonElement>(json);
                    var id = doc.GetProperty("Id").GetGuid();
                    var status = doc.GetProperty("Status").GetString();

                    using var scope = _scopeFactory.CreateScope();
                    var repo = scope.ServiceProvider.GetRequiredService<Persistence.IPropostaStatusRepository>();
                    repo.UpsertStatusAsync(id, status ?? "").GetAwaiter().GetResult();
                }
                catch
                {
                    // ignore
                }
            };

            _channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            try { _channel?.Close(); } catch { }
            try { _connection?.Close(); } catch { }
            return base.StopAsync(cancellationToken);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken) => Task.CompletedTask;
    }
}
