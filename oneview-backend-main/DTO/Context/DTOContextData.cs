namespace DTO.Context {
    public class DTOContextData {
        public required string KeyName { get; set; }
        public string? Value { get; set; }
        public required DTOContextDataType ContextDataType { get; set; }
    }
}