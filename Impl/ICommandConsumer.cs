using System.Threading.Tasks;

namespace PayQueue.Impl
{
    public interface ICommandConsumer<T>
    {
        Task ConsumeCommand(MessageContext<T> message);
    }
}