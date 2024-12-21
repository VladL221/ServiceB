using ServiceB.Interfacees;
using ServiceB.Models;
using System.Net.WebSockets;
using System.Text.Json;
using System.Text;

public class WebSocketMessageHandler : IWebSocketMessageHandler
{
    private readonly IGraphOperationHandler _graphOperationHandler;
    private readonly ILogger<WebSocketMessageHandler> _logger;

    public WebSocketMessageHandler(
        IGraphOperationHandler graphOperationHandler,
        ILogger<WebSocketMessageHandler> logger)
    {
        _graphOperationHandler = graphOperationHandler;
        _logger = logger;
    }

    public async Task HandleMessageAsync(WebSocket webSocket, string message)
    {
        try
        {
            _logger.LogInformation($"Raw message received: {message}");
            var messageObj = JsonSerializer.Deserialize<WebSocketMessage>(message);
            _logger.LogInformation($"Deserialized action: {messageObj?.Action}");
            _logger.LogInformation($"After ToLower: {messageObj?.Action?.ToLower()}");
            if (messageObj == null)
            {
                await SendErrorResponseAsync(webSocket, "Invalid message format");
                return;
            }

            switch (messageObj.Action?.ToLower())
            {
                case "create":
                    var createGraph = JsonSerializer.Deserialize<Graph>(messageObj.Data?.ToString() ?? "");
                    if (createGraph != null)
                    {
                        await _graphOperationHandler.HandleCreateAsync(createGraph);
                        await SendSuccessResponseAsync(webSocket, "Graph created successfully");
                    }
                    break;

                case "get":
                    try
                    {
                        _logger.LogInformation($"Received GET request with data: {messageObj.Data}");
                        if (Guid.TryParse(messageObj.Data?.ToString(), out var getId))
                        {
                            _logger.LogInformation($"Parsed GUID: {getId}");
                            var graph = await _graphOperationHandler.HandleGetAsync(getId);
                            _logger.LogInformation($"Retrieved graph: {JsonSerializer.Serialize(graph)}");

                            if (graph == null)
                            {
                                await SendResponseAsync(webSocket, new WebSocketResponse
                                {
                                    Success = false,
                                    Message = "Graph not found"
                                });
                                return;
                            }

                            await SendResponseAsync(webSocket, new WebSocketResponse
                            {
                                Success = true,
                                Data = new
                                {
                                    Id = graph.Id,
                                    Node = new
                                    {
                                        Id = graph.Node.Id,
                                        Label = graph.Node.Label
                                    },
                                    Edges = graph.Edges.Select(e => new
                                    {
                                        Id = e.Id,
                                        SourceNodeId = e.SourceNodeId,
                                        TargetNodeId = e.TargetNodeId,
                                        Label = e.Label
                                    }).ToList()
                                }
                            });
                        }
                        else
                        {
                            _logger.LogWarning($"Invalid GUID format: {messageObj.Data}");
                            await SendErrorResponseAsync(webSocket, "Invalid graph ID format");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing GET request");
                        await SendErrorResponseAsync(webSocket, "Error processing request");
                    }
                    break;

                case "update":
                    var updateGraph = JsonSerializer.Deserialize<Graph>(messageObj.Data?.ToString() ?? "");
                    if (updateGraph != null)
                    {
                        await _graphOperationHandler.HandleUpdateAsync(updateGraph);
                        await SendSuccessResponseAsync(webSocket, "Graph updated successfully");
                    }
                    break;

                case "delete":
                    if (Guid.TryParse(messageObj.Data?.ToString(), out var deleteId))
                    {
                        await _graphOperationHandler.HandleDeleteAsync(deleteId);
                        await SendSuccessResponseAsync(webSocket, "Graph deleted successfully");
                    }
                    break;

                default:
                    await SendErrorResponseAsync(webSocket, "Unknown action");
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling WebSocket message");
            await SendErrorResponseAsync(webSocket, "Internal server error");
        }
    }

    private async Task SendResponseAsync(WebSocket webSocket, WebSocketResponse response)
    {
        var json = JsonSerializer.Serialize(response);
        var buffer = Encoding.UTF8.GetBytes(json);
        await webSocket.SendAsync(
            new ArraySegment<byte>(buffer),
            WebSocketMessageType.Text,
            true,
            CancellationToken.None);
    }

    private Task SendSuccessResponseAsync(WebSocket webSocket, string message)
        => SendResponseAsync(webSocket, new WebSocketResponse { Success = true, Message = message });

    private Task SendErrorResponseAsync(WebSocket webSocket, string error)
        => SendResponseAsync(webSocket, new WebSocketResponse { Success = false, Message = error });
}