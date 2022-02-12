using System;
using System.Collections.Generic;

namespace CoolStore.AppContracts.Dtos
{
    public class ProductDto
    {
        public string Name { get; set; } = default(string)!;
        public bool Active { get; set; }
        public int Quantity { get; set; }
        public decimal Cost { get; set; }
        public Guid ProductCodeId { get; set; }
        public ProductCodeDto Code { get; set; } = default(ProductCodeDto)!;
        public List<ReturnDto> Returns { get; init; } = new List<ReturnDto>();
        public Guid Id { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
    }
}
