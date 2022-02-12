using System;

namespace CoolStore.AppContracts.Dtos
{
    public class CustomerDto
    {
        public string FirstName { get; set; } = default(string)!;
        public string LastName { get; set; } = default(string)!;
        public string Email { get; set; } = default(string)!;
        public decimal Balance { get; set; }
        public Guid CountryId { get; set; }
        public Guid Id { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
    }
}
