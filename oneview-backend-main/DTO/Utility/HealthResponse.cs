namespace DTO.Utility {

    public class HealthResponse {
        public required string ApiStatus { get; set; }
        public required string DbStatus { get; set; }
        public required DateTime CheckedAtUtc { get; set; }
        public IDictionary<string, string> Details { get; set; } = new Dictionary<string, string>();
    }


}