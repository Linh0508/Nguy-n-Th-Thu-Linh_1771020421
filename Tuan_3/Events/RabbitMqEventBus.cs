using MasterDBank.Events;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Text.Json;

namespace HaiChanBank.Events
{
    // RabbitMQ-backed EventBus (skeleton)
    public class RabbitMqEventBus : IEventBus, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly string _exchangeName = "haichan.exchange";
        private bool _disposed;

        public RabbitMqEventBus(string hostName = "localhost", int port = 5672, string user = "guest", string pass = "guest")
        {
            try
            {
                var factory = new ConnectionFactory()
                {
                    HostName = hostName,
                    Port = port,
                    UserName = user,
                    Password = pass
                };

                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();
                _channel.ExchangeDeclare(_exchangeName, ExchangeType.Topic, durable: true, autoDelete: false);
            }
            catch (Exception ex)
            {
                // Hiển thị lỗi khởi tạo connection để debug
                Console.WriteLine($"[RabbitMqEventBus] Connection init error: {ex.Message}");
                throw;
            }
        }

        public void Publish<T>(T @event) where T : IEvent
        {
            var routingKey = typeof(T).Name;
            var json = JsonSerializer.Serialize(@event, @event.GetType());
            var body = Encoding.UTF8.GetBytes(json);

            var props = _channel.CreateBasicProperties();
            props.Persistent = true;
            props.ContentType = "application/json";

            _channel.BasicPublish(exchange: _exchangeName,
                                  routingKey: routingKey,
                                  basicProperties: props,
                                  body: body);
        }

        public void Subscribe<T>(Action<T> handler) where T : IEvent
        {
            var routingKey = typeof(T).Name;
            var queueName = $"{routingKey}.queue";

            _channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
            _channel.QueueBind(queue: queueName, exchange: _exchangeName, routingKey: routingKey);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (sender, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var json = Encoding.UTF8.GetString(body);
                    var obj = JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if (obj != null)
                    {
                        handler(obj);
                    }
                    _channel.BasicAck(ea.DeliveryTag, multiple: false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[RabbitMqEventBus] handler error: {ex.Message}");
                    _channel.BasicNack(ea.DeliveryTag, multiple: false, requeue: true);
                }
            };

            _channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
        }

        public void Dispose()
        {
            if (_disposed) return;
            try
            {
                _channel?.Close();
                _channel?.Dispose();
                _connection?.Close();
                _connection?.Dispose();
            }
            catch { }
            _disposed = true;
        }
    }
}