namespace DataAccess.Model
{
    internal class Context
    {
        public int Id { get; set; }
        public int? CustomerId { get; set; }
        public Customer? Customer { get; set; }
        public required string Name { get; set; }
        public string? ReferenceLink { get; set; }
        public required DateTime CreatedAt { get; set; }
        public required DateTime LastUpdated { get; set; }
        public required int ContextTypeId { get; set; }
        public required ContextType ContextType { get; set; }
        public List<ContextData> ContextData { get; set; } = new List<ContextData>();
    }
}
