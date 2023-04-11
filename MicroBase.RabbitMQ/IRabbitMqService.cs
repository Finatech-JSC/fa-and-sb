using MicroBase.RabbitMQ.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MicroBase.RabbitMQ
{
    public interface IRabbitMqService
    {
        void FanoutPublic(string exchangeName, byte[] message);

        void FanoutSubscribe(string exchangeName, EventHandler<BasicDeliverEventArgs> eventHandler);
    }

    public class RabbitMqService : IRabbitMqService
    {
        private static ConnectionFactory factory;
        private static IConnection connection;
        private static IModel channel;

        public RabbitMqService(RabbitMqSetting settingModel)
        {
            if (settingModel != null && !string.IsNullOrWhiteSpace(settingModel.HostName))
            {
                factory = new ConnectionFactory()
                {
                    HostName = settingModel.HostName,
                    UserName = settingModel.UserName,
                    Password = settingModel.Password
                };

                connection = factory.CreateConnection();
                channel = connection.CreateModel();
            }
        }

        public void FanoutPublic(string exchangeName, byte[] body)
        {
            channel.ExchangeDeclare(exchange: exchangeName, type: "fanout");
            channel.BasicPublish(exchange: exchangeName,
                                 routingKey: "",
                                 basicProperties: null,
                                 body: body);
        }

        public void FanoutSubscribe(string exchangeName, EventHandler<BasicDeliverEventArgs> eventHandler)
        {
            channel.ExchangeDeclare(exchange: exchangeName, type: "fanout");
            var queueName = channel.QueueDeclare().QueueName;
            channel.QueueBind(queue: queueName,
                              exchange: exchangeName,
                              routingKey: "");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += eventHandler;
            channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
        }
    }
}