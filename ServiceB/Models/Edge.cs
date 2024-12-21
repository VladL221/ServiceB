namespace ServiceB.Models
{
    public record Edge
    {
        public Guid Id { get; init; }
        public Guid SourceNodeId { get; init; }
        public Guid TargetNodeId { get; init; }
        public string Label { get; init; } = string.Empty;
    }
}
