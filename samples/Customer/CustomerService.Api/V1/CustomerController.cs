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
        //#######################################
        //# CUSTOMER TEST
        //#######################################

        [ApiVersion("1.0")]
        [HttpGet("/api/v{version:apiVersion}/customers/test")]
        public async Task<ActionResult> HandleTestAsync()
        {
            return Ok(
                new
                {
                    message = $"Ping {nameof(CustomerController)} OK"
                });
        }

        //#######################################
        //# CUSTOMER MAIN METHODS
        //#######################################

        [ApiVersion("1.0")]
        [HttpPost("/api/v{version:apiVersion}/customers")]
        public async Task<ActionResult> HandleCreateCustomerAsync(
            [FromBody] CreateCustomer.CreateCustomerModel request,
            CancellationToken cancellationToken = new CancellationToken())
        {
            CreateCustomer.Command command = new CreateCustomer.Command
            {
                Model = request
            };

            return Ok(await Mediator.Send(command, cancellationToken));
        }

        [ApiVersion("1.0")]
        [HttpPatch("/api/v{version:apiVersion}/customers")]
        public async Task<ActionResult> HandleUpdateCustomerAsync(
            [FromBody]
            (SendSomeBalanceForCustomer.UpdateCustomerModel model, SendSomeBalanceForCustomer.UpdateCustomerQuery filter) request,
            CancellationToken cancellationToken = new CancellationToken())
        {
            SendSomeBalanceForCustomer.Command command = new SendSomeBalanceForCustomer.Command
            {
                Filter = request.filter, Model = request.model
            };

            return Ok(await Mediator.Send(command, cancellationToken));
        }
    }
}
