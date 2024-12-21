using ServiceB.Models;

namespace ServiceB.Interfacees
{
    public interface IStorageStrategy
    {
        Task<Graph?> GetGraphAsync(Guid id);
        Task<IEnumerable<Graph>> GetAllGraphsAsync();
        Task SaveGraphAsync(Graph graph);
        Task DeleteGraphAsync(Guid id);
        Task<IEnumerable<Graph>> GetDescendantsAsync(Guid nodeId);
    }
}
