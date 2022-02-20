using System.Threading;
using System.Threading.Tasks;
using IdentityService.AppCore.UseCases.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using N8T.Infrastructure.Controller;

namespace IdentityService.Api.V1
{
    [ApiVersion("1.0")]
    public class IdentityController : BaseController
    {
        //#######################################
        //# AUTH TEST
        //#######################################

        [ApiVersion("1.0")]
        [HttpGet("/api/v{version:apiVersion}/auth/test")]
        public async Task<ActionResult> HandleTestAsync()
        {
            return Ok(new { message = $"Ping {nameof(IdentityController)} OK" });
        }

        //#######################################
        //# AUTH MAIN METHODS
        //#######################################

        [AllowAnonymous]
        [HttpPost("/api/v{version:apiVersion}/register")]
        public async Task<ActionResult> HandleRegisterAsync([FromBody] RegisterUser.RegisterUserModel request,
            CancellationToken cancellationToken = new CancellationToken())
        {
            RegisterUser.Command command = new RegisterUser.Command
            {
                Model = request
            };
            return Ok(await Mediator.Send(command, cancellationToken));
        }

        [AllowAnonymous]
        [HttpPost("/api/v{version:apiVersion}/auth")]
        public async Task<ActionResult> HandleAuthenticateAsync([FromBody] Authenticate.AuthenticateModel request,
            CancellationToken cancellationToken = new CancellationToken())
        {
            Authenticate.Command command = new Authenticate.Command
            {
                Model = request
            };
            return Ok(await Mediator.Send(command, cancellationToken));
        }
    }
}
