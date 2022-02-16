using System;

namespace AppContracts.Dtos
{
    public class ReturnDto
    {
        public Guid ProductId { get; set; }
        public ProductDto Product { get; set; } = default(ProductDto)!;
        public Guid CustomerId { get; set; }
        public string Reason { get; set; } = default(string)!;
        public string Note { get; set; } = default(string)!;
        public Guid Id { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
    }
}
