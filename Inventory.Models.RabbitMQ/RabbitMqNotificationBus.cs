using Inventory.Configuration;
using Inventory.Models.DomainEvents;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Inventory.Models.RabbitMQ
{
    public class RabbitMqNotificationBus : IEventStore, IDisposable
    {
        ILogger _domainEventLogger;
        ConnectionFactory _factory;
        IConnection _connection;
        IModel _channel;

        public RabbitMqNotificationBus()
        {
            //Loggin for domain events is configured separatedly from other loggin in the web application
            // to be able to write to a different sink

            _domainEventLogger = new LoggerConfiguration()
                .WriteTo.Debug()
                .CreateLogger();

            if (AppConfiguration.Settings.UseLocalRabbit)
            {
                _factory = new ConnectionFactory()
                {
                    HostName = AppConfiguration.Settings.LocalRabbitIP
                };
            }
            else
            {
                //This configuration should be in an external configuration file and not hardcoded
                _factory = new ConnectionFactory()
                {
                    HostName = "penguin.rmq.cloudamqp.com",
                    VirtualHost = "myhgkegd",
                    UserName = "myhgkegd",
                    Password = "T5pn4hJnBKgC4oVB8G0EKWtj6TIQa2-p"
                };
            }
        }
        public void Publish<T>(T e) where T : DomainEvent
        {
            var typeName = typeof(T).Name;
            _domainEventLogger.Information(e.ToString());
            using (var connection = _factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    string queueName = GetQueueName<T>();

                    //We will use simple queue configuration. In a production
                    // enviroment it will be neccesary to implement a more 
                    // complex configuration with exchanges

                    channel.QueueDeclare(queueName, true, false, false, null);
                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;
                    properties.ContentType = "application/json";
                    properties.Type = typeName;

                    var body = e.ToMessageBody();
                    channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: properties, body: body);
                }
            }
            _domainEventLogger.Information(typeName + " notification sent");
        }

        //Use this method ONLY inside Specflow tests because it disposes connections 
        // inmediately after the first notification arrives
        public async Task SubscribeAndWaitFirstMessage<T>(Action<T> callback) where T : DomainEvent
        {
            SemaphoreSlim signal = new SemaphoreSlim(0, 1);
            string queueName = GetQueueName<T>();

            using (var connection = _factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queueName, true, false, false, null);
                channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    try
                    {
                        T item = ea.Body.ToDomainEvent<T>();
                        callback.Invoke(item);
                    }
                    finally
                    {
                        signal.Release();
                    }
                };
                channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
                await signal.WaitAsync();
            }
        }

        public void Subscribe<T>(Action<T> callback) where T : DomainEvent
        {
            string queueName = GetQueueName<T>();

            if (_connection == null)
                _connection = _factory.CreateConnection();
            if (_channel == null)
                _channel = _connection.CreateModel();

            _channel.QueueDeclare(queueName, true, false, false, null);
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                T item = ea.Body.ToDomainEvent<T>();
                callback.Invoke(item);
            };
            _channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
        }


        private static string GetQueueName<T>() where T : DomainEvent
        {
            var typeName = typeof(T).Name;
            return typeName + "_queue";
        }

        public void Dispose()
        {
            if (_channel != null)
                _channel.Dispose();

            if (_connection != null)
                _connection.Dispose();
        }
    }
}
