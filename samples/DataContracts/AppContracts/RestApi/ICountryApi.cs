using System;
using System.Threading.Tasks;
using AppContracts.Common;
using AppContracts.Dtos;
using RestEase;

namespace AppContracts.RestApi
{
    public interface ICountryApi
    {
        [Get("api/v1/countries/{countryId}")]
        Task<ResultDto<CountryDto>> GetCountryByIdAsync([Path] Guid countryId);
    }
}
