using System.Threading.Tasks;
using PayQueue;
using PayQueue.QueueInterfaces;

namespace PayQueue.Consuming
{
    internal interface IExecutor
    {
        Task Execute(IExchangePublisher exchangePublisher, byte[] data);
    }
}