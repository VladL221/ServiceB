using ServiceB.Interfacees;
using ServiceB.Models;
using System.Text.Json;

namespace ServiceB.Services
{
    public class JsonStorageStrategy : IStorageStrategy
    {
        private readonly string _filePath = "graphs.json";
        private readonly SemaphoreSlim _semaphore = new(1, 1);
        private readonly ILogger<JsonStorageStrategy> _logger;

        public JsonStorageStrategy(ILogger<JsonStorageStrategy> logger)
        {
            _logger = logger;
            if (!File.Exists(_filePath))
            {
                File.WriteAllText(_filePath, "[]");
            }
        }

        public async Task<Graph?> GetGraphAsync(Guid id)
        {
            await _semaphore.WaitAsync();
            try
            {
                var graphs = await LoadGraphsAsync();
                return graphs.FirstOrDefault(g => g.Id == id);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<IEnumerable<Graph>> GetAllGraphsAsync()
        {
            await _semaphore.WaitAsync();
            try
            {
                return await LoadGraphsAsync();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task SaveGraphAsync(Graph graph)
        {
            await _semaphore.WaitAsync();
            try
            {
                var graphs = (await LoadGraphsAsync()).ToList();
                var existingIndex = graphs.FindIndex(g => g.Id == graph.Id);

                if (existingIndex != -1)
                {
                    graphs[existingIndex] = graph;
                }
                else
                {
                    graphs.Add(graph);
                }

                await SaveGraphsAsync(graphs);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task DeleteGraphAsync(Guid id)
        {
            await _semaphore.WaitAsync();
            try
            {
                var graphs = (await LoadGraphsAsync()).ToList();
                graphs.RemoveAll(g => g.Id == id);
                await SaveGraphsAsync(graphs);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<IEnumerable<Graph>> GetDescendantsAsync(Guid nodeId)
        {
            await _semaphore.WaitAsync();
            try
            {
                var allGraphs = await LoadGraphsAsync();
                var descendants = new HashSet<Graph>();
                var visited = new HashSet<Guid>();

                async Task TraverseDescendants(Guid currentNodeId)
                {
                    if (visited.Contains(currentNodeId))
                        return;

                    visited.Add(currentNodeId);

                    var connectedGraphs = allGraphs.Where(g =>
                        g.Edges.Any(e => e.SourceNodeId == currentNodeId));

                    foreach (var graph in connectedGraphs)
                    {
                        descendants.Add(graph);
                        foreach (var edge in graph.Edges)
                        {
                            await TraverseDescendants(edge.TargetNodeId);
                        }
                    }
                }

                await TraverseDescendants(nodeId);
                return descendants;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<IEnumerable<Graph>> LoadGraphsAsync()
        {
            try
            {
                var json = await File.ReadAllTextAsync(_filePath);
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true 
                };
                return JsonSerializer.Deserialize<IEnumerable<Graph>>(json, options) ?? new List<Graph>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading graphs from file");
                return new List<Graph>();
            }
        }

        private async Task SaveGraphsAsync(IEnumerable<Graph> graphs)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
                };
                var json = JsonSerializer.Serialize(graphs, options);
                await File.WriteAllTextAsync(_filePath, json);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving graphs to file");
                throw;
            }
        }

    }
}
