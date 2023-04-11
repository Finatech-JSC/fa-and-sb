using MicroBase.RabbitMQ.Models;

namespace MicroBase.RabbitMQ
{
    public interface IRabbitConnectionProvider
    {
        string GetConnectionString();
    }

    public class RabbitConnectionProvider : IRabbitConnectionProvider
    {
        private readonly RabbitMqSetting rabbitMqSetting;

        public RabbitConnectionProvider(RabbitMqSetting settingModel)
        {
            rabbitMqSetting = settingModel;
        }

        public string GetConnectionString()
        {
            if (rabbitMqSetting == null)
            {
                return string.Empty;
            }

            var conn = $"host={rabbitMqSetting.HostName};virtualHost={rabbitMqSetting.VirtualHost}";
            if (!string.IsNullOrWhiteSpace(rabbitMqSetting.UserName))
            {
                conn += $";username={rabbitMqSetting.UserName}";
            }

            if (!string.IsNullOrWhiteSpace(rabbitMqSetting.Password))
            {
                conn += $";password={rabbitMqSetting.Password}";
            }

            return conn;
        }
    }
}