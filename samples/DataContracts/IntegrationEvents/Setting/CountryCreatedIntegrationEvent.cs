using System;
using N8T.Core.Domain;

namespace IntegrationEvents.Setting
{
    public class CountryCreatedIntegrationEvent : EventBase
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default(string)!;

        public override void Flatten()
        {
            MetaData.Add("Id", Id);
            MetaData.Add("Name", Name);
        }
    }
}
