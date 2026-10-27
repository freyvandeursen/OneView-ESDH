namespace DataAccess.Model {
    internal class Contact {
        public required int Id { get; set; }
        public required string CustomerId { get; set; }
        public required string Name { get; set; }
        public string? Email { get; set; }
        public string? Title { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
