namespace ServiceB.Models
{
    public record Node
    {
        public Guid Id { get; init; }
        public string Label { get; init; } = string.Empty;
    }
}
