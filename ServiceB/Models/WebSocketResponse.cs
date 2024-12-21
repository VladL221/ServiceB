namespace ServiceB.Models
{
    public record WebSocketResponse
    {
        public bool Success { get; init; }
        public string? Message { get; init; }
        public object? Data { get; init; }
        public string CorrelationId { get; internal set; }
    }
}
