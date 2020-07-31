

namespace PayQueue.Definition 
{
    public interface IServiceDefinition
    {
        void Configure(IConfigurator conf);
        string Label();
    }
}