using System;
using System.Threading;
using System.Threading.Tasks;
using CoolStore.AppContracts.Dtos;
using CoolStore.AppContracts.RestApi;
using CustomerService.AppCore.Core.Entities;
using CustomerService.AppCore.Core.Specs;
using FluentValidation;
using MediatR;
using N8T.Core.Domain;
using N8T.Core.Repository;

namespace CustomerService.AppCore.UseCases.Commands
{
    public class CreateCustomer
    {
        public record CreateCustomerModel(string FirstName, string LastName, string Email, Guid CountryId);
        public record Command : ICreateCommand<CreateCustomerModel, CustomerDto>
        {
            public CreateCustomerModel Model { get; init; } = default(CreateCustomerModel)!;

            internal class Validator : AbstractValidator<Command>
            {
                public Validator()
                {
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

            internal class Handler : IRequestHandler<Command, ResultModel<CustomerDto>>
            {
                private readonly ICountryApi _countryApi;
                private readonly IRepository<Customer> _customerRepository;

                public Handler(IRepository<Customer> customerRepository, ICountryApi countryApi)
                {
                    _customerRepository =
                        customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
                    _countryApi = countryApi ?? throw new ArgumentNullException(nameof(countryApi));
                }

                public async Task<ResultModel<CustomerDto>> Handle(Command request,
                    CancellationToken cancellationToken)
                {
                    CustomerAlreadyRegisteredSpec alreadyRegisteredSpec =
                        new CustomerAlreadyRegisteredSpec(request.Model.Email);

                    Customer? existingCustomer = await _customerRepository.FindOneAsync(alreadyRegisteredSpec);

                    if (existingCustomer != null)
                    {
                        throw new Exception("Customer with this email already exists");
                    }

                    // check country is exists and valid
                    (CountryDto countryDto, bool isError, _) =
                        await _countryApi.GetCountryByIdAsync(request.Model.CountryId);
                    if (isError || countryDto.Id.Equals(Guid.Empty))
                    {
                        throw new Exception("Country Id is not valid.");
                    }

                    Customer customer = Customer.Create(request.Model.FirstName, request.Model.LastName,
                        request.Model.Email, request.Model.CountryId);

                    //customer.AddDomainEvent(new CustomerCreatedIntegrationEvent());

                    Customer? created = await _customerRepository.AddAsync(customer);

                    return ResultModel<CustomerDto>.Create(new CustomerDto
                    {
                        Id = created.Id,
                        FirstName = created.FirstName,
                        LastName = created.LastName,
                        Email = created.Email,
                        CountryId = created.CountryId,
                        Balance = created.Balance,
                        Created = created.Created,
                        Updated = created.Updated
                    });
                }
            }
        }
    }
}
