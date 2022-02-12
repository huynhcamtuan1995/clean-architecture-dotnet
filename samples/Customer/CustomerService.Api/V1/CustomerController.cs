using System.Threading;
using System.Threading.Tasks;
using CustomerService.AppCore.UseCases.Commands;
using Microsoft.AspNetCore.Mvc;
using N8T.Infrastructure.Controller;

namespace CustomerService.Application.V1
{
    [ApiVersion("1.0")]
    public class CustomerController : BaseController
    {
        [ApiVersion("1.0")]
        [HttpPost("/api/v{version:apiVersion}/customers")]
        public async Task<ActionResult> HandleCreateCustomerAsync([FromBody]
            CreateCustomer.CreateCustomerModel request,
            CancellationToken cancellationToken = new CancellationToken())
        {
            CreateCustomer.Command command = new CreateCustomer.Command
            {
                Model = request
            };
            return Ok(await Mediator.Send(command, cancellationToken));
        }
    }
}
