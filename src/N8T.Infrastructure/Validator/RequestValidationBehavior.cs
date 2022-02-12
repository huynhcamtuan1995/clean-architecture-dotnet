using System;
using System.Diagnostics;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace N8T.Infrastructure.Validator
{
    [DebuggerStepThrough]
    public class RequestValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull, IRequest<TResponse>
        where TResponse : notnull
    {
        private readonly ILogger<RequestValidationBehavior<TRequest, TResponse>> _logger;
        private readonly IValidator<TRequest> _validator;

        public RequestValidationBehavior(IValidator<TRequest> validator,
            ILogger<RequestValidationBehavior<TRequest, TResponse>> logger)
        {
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<TResponse> Handle(TRequest request,
            CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
        {
            _logger.LogInformation(
                "[{Prefix}] Handle request={X-RequestData} and response={X-ResponseData}",
                nameof(RequestValidationBehavior<TRequest, TResponse>), typeof(TRequest).Name, typeof(TResponse).Name);

            _logger.LogDebug($"Handling {typeof(TRequest).FullName} with content {JsonSerializer.Serialize(request)}");

            await _validator.HandleValidation(request);

            TResponse response = await next();

            _logger.LogInformation($"Handled {typeof(TRequest).FullName}");
            return response;
        }
    }
}
