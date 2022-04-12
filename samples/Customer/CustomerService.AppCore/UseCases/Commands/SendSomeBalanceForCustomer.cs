using System;
using System.Threading;
using System.Threading.Tasks;
using AppContracts.RestApi;
using CustomerService.AppCore.Core.Entities;
using CustomerService.AppCore.Core.Specs;
using FluentValidation;
using MediatR;
using N8T.Core.Domain;
using N8T.Core.Repository;

namespace CustomerService.AppCore.UseCases.Commands
{
    public class SendSomeBalanceForCustomer
    {
        public record UpdateCustomerModel(string FirstName, string LastName, string Email, decimal Amount, Guid CountryId);

        public record UpdateCustomerQuery(Guid CountryId);

        public sealed record Command : IUpdateCommand<UpdateCustomerQuery, UpdateCustomerModel, bool>
        {
            public UpdateCustomerQuery Filter { get; init; } = default(UpdateCustomerQuery)!;
            public UpdateCustomerModel Model { get; init; } = default(UpdateCustomerModel)!;
        }

        internal class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(v => v.Filter.CountryId)
                    .NotNull()
                    .WithMessage("CountryId is required.");

                RuleFor(v => v.Model.FirstName)
                    .NotEmpty()
                    .WithMessage("FirstName is required.")
                    .MaximumLength(50)
                    .WithMessage("FirstName must not exceed 50 characters.");

                RuleFor(v => v.Model.LastName)
                    .NotEmpty()
                    .WithMessage("LastName is required.")
                    .MaximumLength(50)
                    .WithMessage("LastName must not exceed 50 characters.");

                RuleFor(v => v.Model.Amount)
                    .NotNull()
                    .WithMessage("Amount is required.")
                    .GreaterThan(0)
                    .WithMessage("Amount cannot be negative.");

                RuleFor(v => v.Model.Email)
                    .NotEmpty()
                    .WithMessage("Email is required.")
                    .EmailAddress()
                    .WithMessage("Email should in email format.")
                    .MaximumLength(100)
                    .WithMessage("Email must not exceed 100 characters.");
            }
        }

        internal class Handler : IRequestHandler<Command, ResultModel<bool>>
        {
            private readonly ICountryApi _countryApi;
            private readonly IRepository<Customer> _customerRepository;

            public Handler(IRepository<Customer> customerRepository, ICountryApi countryApi)
            {
                _customerRepository =
                    customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));

                _countryApi = countryApi ?? throw new ArgumentNullException(nameof(countryApi));
            }

            public async Task<ResultModel<bool>> Handle(
                Command request,
                CancellationToken cancellationToken)
            {
                //customer.AddDomainEvent(new CustomerCreatedIntegrationEvent());
                NonBannedCountriesSpec spec = new NonBannedCountriesSpec(request.Filter.CountryId);
                Customer customer = Customer.Create(
                    request.Model.FirstName,
                    request.Model.LastName,
                    request.Model.Email,
                    request.Model.CountryId);
                customer.AddAmount(request.Model.Amount); // Add amount

                bool isUpdate = await _customerRepository.UpdateAsync(spec, customer);

                return ResultModel<bool>.Create(isUpdate);
            }
        }
    }
}
