

namespace DataAccess.Model {
    internal class Label {
        public int Id { get; set; }
        public required string CustomerId { get; set; }
        public int? ContextId { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public string? Value { get; set; }
    }
}
