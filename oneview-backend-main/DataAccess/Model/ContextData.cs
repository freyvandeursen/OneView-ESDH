namespace DataAccess.Model {
    internal class ContextData {
        public int Id { get; set; }
        public required string KeyName { get; set; }
        public string? Value { get; set; }
        public required int ContextId { get; set; }
        public required Context Context { get; set; }
        public required int ContextDataTypeId { get; set; }
        public required ContextDataType ContextDataType { get; set; }
    }
}