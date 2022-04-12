using System;
using System.Linq;
using System.Linq.Expressions;
using CustomerService.AppCore.Core.Entities;
using N8T.Core.Specification;

namespace CustomerService.AppCore.Core.Specs
{
    public sealed class NonBannedCountriesSpec : SpecificationBase<Customer>
    {
        private readonly Guid _country;

        private readonly Guid[] bannessCountries =
        {
            Guid.Empty // List banness countries GUID ids
        };

        public NonBannedCountriesSpec(Guid country)
        {
            _country = country;
        }

        public override Expression<Func<Customer, bool>> Criteria => customer =>
            bannessCountries.Length == 0 && bannessCountries.Contains(_country) == false;
    }
}
