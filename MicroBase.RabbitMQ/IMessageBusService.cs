using EasyNetQ;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MicroBase.RabbitMQ
{
    public interface IMessageBusService
    {
        void Send<TRequest>(string queueName, TRequest request) where TRequest : class;

        Task SendAsync<TRequest>(string queueName, TRequest request) where TRequest : class;

        Task Receive<TRequest>(string queueName, IMessageHandler<TRequest> handler) where TRequest : class;

        void Subscribe<TRequest>(string subscriptionId, IMessageHandler<TRequest> handler) where TRequest : class;

        void Publish<TRequest>(string subscriptionId, TRequest message) where TRequest : class;

        void InitQueues(IEnumerable<string> queues);
    }

    public class MessageBusService : IMessageBusService
    {
        private readonly IBus bus;
        private readonly ILogger logger;
        private readonly bool ENABLED_RABBITMQ = false;

        public MessageBusService(ILoggerFactory loggerFactory,
            IRabbitConnectionProvider connectionProvider,
            IConfiguration configuration)
        {
            ENABLED_RABBITMQ = configuration.GetValue<bool>("RabbitMq:IsEnabled");

            this.logger = loggerFactory.CreateLogger<MessageBusService>();

            var connectionStr = connectionProvider.GetConnectionString();
            if (!string.IsNullOrWhiteSpace(connectionStr))
            {
                bus = RabbitHutch.CreateBus(connectionStr);
            }
        }

        public void Send<TRequest>(string queueName, TRequest request) where TRequest : class
        {
            if (!ENABLED_RABBITMQ)
            {
                return;
            }

            try
            {
                bus.SendReceive.Send(queueName, request);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task SendAsync<TRequest>(string queueName, TRequest request) where TRequest : class
        {
            if (!ENABLED_RABBITMQ)
            {
                return;
            }

            try
            {
                await bus.SendReceive.SendAsync(queueName, request);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Task Receive<TRequest>(string queueName, IMessageHandler<TRequest> handler) where TRequest : class
        {
            if (!ENABLED_RABBITMQ)
            {
                return Task.CompletedTask;
            }

            bus.SendReceive.Receive<TRequest>(queueName, message =>
            {
                try
                {
                    handler.OnMessageReceivedAsync(message);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex.ToString());
                }
            });

            return Task.CompletedTask;
        }

        public void Subscribe<TRequest>(string subscriptionId, IMessageHandler<TRequest> handler) where TRequest : class
        {
            if (!ENABLED_RABBITMQ)
            {
                return;
            }

            bus.PubSub.Subscribe<TRequest>(subscriptionId, message =>
            {
                try
                {
                    handler.OnMessageReceivedAsync(message);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex.ToString());
                }
            });
        }

        public void Publish<TRequest>(string subscriptionId, TRequest message) where TRequest : class
        {
            if (!ENABLED_RABBITMQ)
            {
                return;
            }

            try
            {
                bus.PubSub.Publish(message);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void InitQueues(IEnumerable<string> queues)
        {
            if (!ENABLED_RABBITMQ)
            {
                return;
            }

            foreach (var item in queues)
            {
                bus.Advanced.QueueDeclare(item);
            }
        }
    }
}