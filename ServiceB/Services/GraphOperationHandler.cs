using ServiceB.Interfacees;
using ServiceB.Models;
using System.Text;
using System.Text.Json;
using System.Net.WebSockets;

namespace ServiceB.Services
{
    public class GraphOperationHandler : IGraphOperationHandler
    {
        private readonly IStorageStrategy _storage;
        private readonly ILogger<GraphOperationHandler> _logger;

        public GraphOperationHandler(IStorageStrategy storage, ILogger<GraphOperationHandler> logger)
        {
            _storage = storage;
            _logger = logger;
        }

        public async Task HandleCreateAsync(Graph graph)
        {
            try
            {
                await _storage.SaveGraphAsync(graph);
                _logger.LogInformation("Created graph with ID: {GraphId}", graph.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating graph");
                throw;
            }
        }

        public async Task<Graph?> HandleGetAsync(Guid id)
        {
            try
            {
                _logger.LogInformation($"Getting graph with ID: {id}");
                var graph = await _storage.GetGraphAsync(id);
                if (graph == null)
                {
                    _logger.LogWarning($"Graph not found with ID: {id}");
                }
                else
                {
                    _logger.LogInformation($"Found graph: {JsonSerializer.Serialize(graph)}");
                }
                return graph;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting graph");
                throw;
            }
        }

        public async Task HandleUpdateAsync(Graph graph)
        {
            try
            {
                await _storage.SaveGraphAsync(graph);
                _logger.LogInformation("Updated graph with ID: {GraphId}", graph.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating graph");
                throw;
            }
        }

        public async Task HandleDeleteAsync(Guid id)
        {
            try
            {
                await _storage.DeleteGraphAsync(id);
                _logger.LogInformation("Deleted graph with ID: {GraphId}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting graph");
                throw;
            }
        }

        public async Task<IEnumerable<Graph>> HandleGetDescendantsAsync(Guid nodeId)
        {
            try
            {
                var descendants = await _storage.GetDescendantsAsync(nodeId);
                _logger.LogInformation("Retrieved descendants for node: {NodeId}", nodeId);
                return descendants;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting descendants");
                throw;
            }
        }
    }
}
