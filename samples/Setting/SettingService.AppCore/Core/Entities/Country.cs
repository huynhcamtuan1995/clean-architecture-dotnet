using System;
using IntegrationEvents.Setting;
using N8T.Core.Domain;

namespace SettingService.AppCore.Core.Entities
{
    public class Country : EntityRootBase
    {
        public string Name { get; protected set; } = default(string)!;

        public static Country Create(string name)
        {
            return Create(Guid.NewGuid(), name);
        }

        public static Country Create(Guid id, string name)
        {
            Country country = new Country { Id = id, Name = name };

            country.AddDomainEvent(new CountryCreatedIntegrationEvent { Id = country.Id, Name = country.Name });

            return country;
        }
    }
}
