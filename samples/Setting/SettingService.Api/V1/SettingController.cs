using System;
using System.Threading;
using System.Threading.Tasks;
using AppContracts.Dtos;
using Microsoft.AspNetCore.Mvc;
using N8T.Infrastructure.Controller;
using SettingService.AppCore.UseCases.Queries;

namespace SettingService.Application.V1
{
    [ApiVersion("1.0")]
    public class SettingController : BaseController
    {
        //#######################################
        //# SETTING TEST
        //#######################################

        [ApiVersion("1.0")]
        [HttpGet("/api/v{version:apiVersion}/countries/test")]
        public async Task<ActionResult> HandleTestAsync()
        {
            return Ok(new { message = $"Ping {nameof(SettingController)} OK" });
        }

        //#######################################
        //# SETTING MAIN METHODS
        //#######################################

        [ApiVersion("1.0")]
        [HttpGet("/api/v{version:apiVersion}/countries/{id:guid}")]
        public async Task<ActionResult<CountryDto>> HandleGetCountryByIdAsync(
            Guid id,
            CancellationToken cancellationToken = new CancellationToken())
        {
            GetCountryById.Query request = new GetCountryById.Query { Data = id };

            return Ok(await Mediator.Send(request, cancellationToken));
        }
    }
}
