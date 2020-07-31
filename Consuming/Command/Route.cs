using PayQueue.Internal;

namespace PayQueue.Consuming.Command
{
    internal class Route
    {
        private readonly Routes _routes;
        public Route(Routes routes, bool appl) => (_routes, IsApplicable) = (routes, appl);
        public string Queue
        {
            get { return _routes.ConsumeCommandQueue(); }
        }
        public bool IsApplicable { get; }
    }
}