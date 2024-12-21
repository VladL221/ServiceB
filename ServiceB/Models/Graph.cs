namespace ServiceB.Models
{
    public record Graph
    {
        public Guid Id { get; init; }
        public Node Node { get; init; } = new();
        public List<Edge> Edges { get; init; } = new();
    }
}
