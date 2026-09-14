using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using InsureFlow.PropostaService.Application.Ports;

namespace InsureFlow.PropostaService.Infrastructure.RabbitMq
{
    public class RabbitMqEventPublisher : IEventPublisher, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public RabbitMqEventPublisher(string hostName)
        {
            var factory = new ConnectionFactory() { HostName = hostName };

            // Retry logic: try to connect to RabbitMQ several times with exponential backoff
            const int maxAttempts = 6;
            var attempt = 0;
            Exception? lastEx = null;
            while (attempt < maxAttempts)
            {
                attempt++;
                try
                {
                    _connection = factory.CreateConnection();
                    _channel = _connection.CreateModel();
                    lastEx = null;
                    break;
                }
                catch (Exception ex)
                {
                    lastEx = ex;
                    // If this was the last attempt, rethrow to preserve previous behavior
                    if (attempt >= maxAttempts)
                        throw;

                    // Backoff: 500ms * 2^(attempt-1)
                    try
                    {
                        var backoffMs = 500 * (1 << (attempt - 1));
                        System.Console.WriteLine($"RabbitMQ connection attempt {attempt} failed. Waiting {backoffMs}ms before retry. Exception: {ex.Message}");
                        System.Threading.Thread.Sleep(backoffMs);
                    }
                    catch
                    {
                        // ignore sleep failures
                    }
                }
            }
        }

        public void Publish(string exchange, string routingKey, object @event)
        {
            _channel.ExchangeDeclare(exchange, ExchangeType.Fanout, durable: true);
            var json = JsonSerializer.Serialize(@event);
            var body = Encoding.UTF8.GetBytes(json);
            _channel.BasicPublish(exchange: exchange, routingKey: routingKey ?? string.Empty, basicProperties: null, body: body);
        }

        public void Dispose()
        {
            try { _channel?.Close(); } catch { }
            try { _connection?.Close(); } catch { }
        }
    }
}
