using System;
using System.Threading;
using System.Threading.Tasks;
using AppContracts.Dtos;
using FluentValidation;
using IdentityService.AppCore.Interfaces;
using Mapster;
using MediatR;
using N8T.Core.Domain;
using N8T.Infrastructure.Auth;

namespace IdentityService.AppCore.UseCases.Commands
{
    public class Authenticate
    {
        public record AuthenticateModel : IAuthenticateModel
        {
            public string UserName { get; set; }
            public string Password { get; set; }
        }

        public record Command : IAuthenticateRequest<AuthenticateModel, AccessTokenDto>
        {
            public AuthenticateModel Model { get; init; } = default(AuthenticateModel)!;
        }

        internal class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(v => v.Model.UserName)
                    .NotEmpty().WithMessage("UserName is required.")
                    .MaximumLength(50).WithMessage("UserName must not exceed 50 characters.");

                RuleFor(v => v.Model.Password)
                    .NotEmpty().WithMessage("Password is required.")
                    .MaximumLength(50).WithMessage("FirstName must not exceed 50 characters.");
            }
        }

        internal class Handler : IRequestHandler<Command, ResultModel<AccessTokenDto>>
        {
            private readonly IIdentityUserService _identityUserService;

            public Handler(IIdentityUserService identityUserService)
            {
                _identityUserService =
                    identityUserService ?? throw new ArgumentNullException(nameof(identityUserService));
            }

            public async Task<ResultModel<AccessTokenDto>> Handle(Command request,
                CancellationToken cancellationToken = default(CancellationToken))
            {
                AuthenticateDto authenticate = request.Model.Adapt<AuthenticateDto>();
                // Call to auth service generate jwt
                AccessTokenDto accessToken = await _identityUserService.AuthenticateAsync(authenticate);
                if (accessToken == null)
                {
                    throw new Exception("Login failed");
                }
                return ResultModel<AccessTokenDto>.Create(accessToken);
            }
        }
    }
}
