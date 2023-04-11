using System.Threading.Tasks;

namespace MicroBase.RabbitMQ
{
    public interface IMessageHandler<in TRequest> where TRequest : class
    {
        Task OnMessageReceivedAsync(TRequest message);
    }
}