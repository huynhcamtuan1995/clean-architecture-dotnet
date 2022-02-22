using System;
using System.Threading;
using System.Threading.Tasks;
using AppContracts.Dtos;
using AppContracts.RestApi;
using FluentValidation;
using IdentityService.AppCore.Interfaces;
using Mapster;
using MediatR;
using N8T.Core.Domain;
using N8T.Infrastructure.Auth;

namespace IdentityService.AppCore.UseCases.Commands
{
    public class RegisterUser
    {
        public record RegisterUserModel(string UserName, string Password, string FirstName, string LastName, string Email);

        public record Command : ICreateCommand<RegisterUserModel, AccessTokenDto>
        {
            public RegisterUserModel Model { get; init; } = default(RegisterUserModel)!;
        }

        internal class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(v => v.Model.Password)
                    .NotEmpty().WithMessage("Password is required.")
                    .MaximumLength(50).WithMessage("Password must not exceed 50 characters.");

                RuleFor(v => v.Model.UserName)
                    .NotEmpty().WithMessage("UserName is required.")
                    .MaximumLength(50).WithMessage("UserName must not exceed 50 characters.");

                RuleFor(v => v.Model.FirstName)
                    .NotEmpty().WithMessage("FirstName is required.")
                    .MaximumLength(50).WithMessage("FirstName must not exceed 50 characters.");

                RuleFor(v => v.Model.LastName)
                    .NotEmpty().WithMessage("LastName is required.")
                    .MaximumLength(50).WithMessage("LastName must not exceed 50 characters.");

                RuleFor(v => v.Model.Email)
                    .NotEmpty().WithMessage("Email is required.")
                    .EmailAddress().WithMessage("Email should in email format.")
                    .MaximumLength(100).WithMessage("Email must not exceed 100 characters.");
            }
        }

        internal class Handler : IRequestHandler<Command, ResultModel<AccessTokenDto>>
        {
            private readonly IIdentityUserService _identityUserService;
            private readonly ICustomerApi _customerApi;

            public Handler(IIdentityUserService identityUserService,
                ICustomerApi customerApi)
            {
                _identityUserService = identityUserService ?? throw new ArgumentNullException(nameof(identityUserService));
                _customerApi = customerApi ?? throw new ArgumentNullException(nameof(customerApi));
            }

            public async Task<ResultModel<AccessTokenDto>> Handle(Command request,
                CancellationToken cancellationToken)
            {
                CustomerDto customerDto = request.Model.Adapt<CustomerDto>();
                (Guid userId, AccessTokenDto accessToken) = await _identityUserService.RegisterAsync(customerDto);
                if (userId == Guid.Empty)
                {
                    throw new Exception("Register failed");
                }

                customerDto.UserId = userId;
                (Guid guid, bool isError, _) = await _customerApi.CreateCustomerAsync(customerDto);
                if (isError || guid == Guid.Empty)
                {
                    throw new Exception("Customer create failed");
                }

                return ResultModel<AccessTokenDto>.Create(accessToken);
            }
        }
    }
}
