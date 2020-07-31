
using System.Threading.Tasks;
using PayQueue.Definition;

namespace PayQueue.Impl
{
    public interface IEventConsumer<S, T> where S: IServiceDefinition, new()
    {
        Task ConsumeEvent(MessageContext<T>  message);
    }
}