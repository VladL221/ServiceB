using System.Net.WebSockets;

namespace ServiceB.Interfacees
{
    public interface IWebSocketMessageHandler
    {
        Task HandleMessageAsync(WebSocket webSocket, string message);
    }
}
