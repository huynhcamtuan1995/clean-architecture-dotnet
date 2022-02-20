using System;
using System.Threading;
using System.Threading.Tasks;
using AppContracts.Dtos;
using FluentValidation;
using IdentityService.AppCore.Interfaces;
using Mapster;
using MediatR;
using N8T.Core.Domain;

namespace IdentityService.AppCore.UseCases.Commands
{
    public class Authenticate
    {
        public record AuthenticateModel(string UserName, string Password);

        public record Command : ICreateCommand<AuthenticateModel, AccessTokenDto>
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
            private readonly IUserIdentityService _userIdentityService;

            public Handler(IUserIdentityService userIdentityService)
            {
                _userIdentityService =
                    userIdentityService ?? throw new ArgumentNullException(nameof(userIdentityService));
            }

            public async Task<ResultModel<AccessTokenDto>> Handle(Command request,
                CancellationToken cancellationToken = default(CancellationToken))
            {
                UserLoginDto userLogin = request.Model.Adapt<UserLoginDto>();
                // Call to auth service generate jwt
                AccessTokenDto accessToken = await _userIdentityService.Authenticate(userLogin);
                if (accessToken == null)
                {
                    throw new Exception("Login failed");
                }
                return ResultModel<AccessTokenDto>.Create(accessToken);
            }
        }
    }
}
