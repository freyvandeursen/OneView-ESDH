namespace DataAccess.Model {
    internal class ContextType {
        public int Id { get; set; }
        public required string TypeName { get; set; }
        public string? Description { get; set; }
    }
}