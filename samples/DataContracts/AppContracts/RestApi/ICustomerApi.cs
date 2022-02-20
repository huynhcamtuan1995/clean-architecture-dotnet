using System;
using System.Threading.Tasks;
using AppContracts.Common;
using AppContracts.Dtos;
using RestEase;

namespace AppContracts.RestApi
{
    public interface ICustomerApi
    {
        [Post("api/v1/customers")]
        Task<ResultDto<Guid>> CreateCustomerAsync([Body] CustomerDto customer);
    }
}
