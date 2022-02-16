using System.Threading.Tasks;
using AppContracts.Common;
using AppContracts.Dtos;
using RestEase;

namespace AppContracts.RestApi
{
    public interface IAppApi
    {
        [Get("api/product-api/v1/products")]
        Task<ResultDto<ListResultDto<ProductDto>>> GetProductsAsync([Header("x-query")] string xQuery);
    }
}
