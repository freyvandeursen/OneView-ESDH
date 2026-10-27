namespace DTO.Context
{
    public class DTOContext
    {
        public required string Name { get; set; }
        public string? ReferenceLink { get; set; }
        public required DTOContextType ContextType { get; set; }
        public List<DTOContextData> ContextData { get; set; } = new();
        public required DateTime LastUpdated { get; set; }
        public required DateTime CreatedAt { get; set; }
        public string? CustomerId { get; set; }
        public int ContextId { get; set; }
    }
}
