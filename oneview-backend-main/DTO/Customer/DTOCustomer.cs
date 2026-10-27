namespace DTO.Customer {
    public class DTOCustomer {
        public required string CustomerId { get; set; }
        public required string Name { get; set; }
        public required string Address { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? VATNumber { get; set; }
        public DateTime? DeactivationDate { get; set; }
    }
}