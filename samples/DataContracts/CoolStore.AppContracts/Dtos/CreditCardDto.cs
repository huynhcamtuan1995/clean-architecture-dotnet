using System;

namespace CoolStore.AppContracts.Dtos
{
    public class CreditCardDto
    {
        public string NameOnCard { get; set; } = default(string)!;
        public string CardNumber { get; set; } = default(string)!;
        public bool Active { get; set; }
        public DateTime Expiry { get; set; }
        public Guid Id { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
    }
}
