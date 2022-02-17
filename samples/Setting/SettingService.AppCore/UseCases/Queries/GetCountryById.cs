using System;
using System.Collections.Generic;
using AppContracts.Dtos;
using FluentValidation;
using MediatR;
using N8T.Core.Domain;
using N8T.Core.Repository;
using SettingService.AppCore.Core.Entities;

namespace SettingService.AppCore.UseCases.Queries
{
    public class GetCountryById
    {
        public record Query : IItemQuery<Guid, CountryDto>
        {
            public List<string> Includes { get; init; } = new List<string>();
            public Guid Data { get; init; }
        }

        internal class Validator : AbstractValidator<Query>
        {
            public Validator()
            {
                RuleFor(x => x.Data)
                    .NotNull()
                    .NotEmpty().WithMessage($"{nameof(Query.Data)} is required.");
            }
        }

        internal class Handler : RequestHandler<GetCountryById.Query, ResultModel<CountryDto>>
        {
            private readonly IRepository<Country> _countryRepository;

            public Handler(IRepository<Country> countryRepository)
            {
                _countryRepository =
                    countryRepository ?? throw new ArgumentNullException(nameof(countryRepository));
            }

            protected override ResultModel<CountryDto> Handle(GetCountryById.Query request)
            {
                if (request == null)
                {
                    throw new ArgumentNullException(nameof(request));
                }

                Country country = _countryRepository.FindById(request.Data);

                return ResultModel<CountryDto>.Create(new CountryDto
                {
                    Id = country.Id,
                    Name = country.Name,
                    Created = country.Created,
                    Updated = country.Updated
                });
            }
        }
    }
}
