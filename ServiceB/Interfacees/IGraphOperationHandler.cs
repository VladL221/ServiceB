using ServiceB.Models;

namespace ServiceB.Interfacees
{
    public interface IGraphOperationHandler
    {
        Task HandleCreateAsync(Graph graph);
        Task<Graph?> HandleGetAsync(Guid id);
        Task HandleUpdateAsync(Graph graph);
        Task HandleDeleteAsync(Guid id);
        Task<IEnumerable<Graph>> HandleGetDescendantsAsync(Guid nodeId);
    }
}
