using System.Text.Json.Serialization;

namespace ServiceB.Models
{
    public class WebSocketMessage
    {
        [JsonPropertyName("action")]
        public string? Action { get; set; }

        [JsonPropertyName("data")]
        public object? Data { get; set; }
    }
}
