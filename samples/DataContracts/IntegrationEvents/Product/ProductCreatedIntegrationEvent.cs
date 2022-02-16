using System;
using N8T.Core.Domain;

namespace IntegrationEvents.Product
{
    public class ProductCreatedIntegrationEvent : EventBase
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default(string)!;
        public int Quantity { get; set; }
        public Guid ProductCodeId { get; set; }
        public decimal ProductCost { get; set; }

        public override void Flatten()
        {
            MetaData.Add("ProductId", Id);
            MetaData.Add("ProductName", Name);
            MetaData.Add("ProductQuantity", Quantity);
            MetaData.Add("ProductCode", ProductCodeId);
            MetaData.Add("ProductCost", ProductCost);
        }
    }
}
