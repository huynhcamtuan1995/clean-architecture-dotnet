using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CustomerService.AppCore.Core.Specs;
using IntegrationEvents.Customer;
using N8T.Core.Domain;

namespace CustomerService.AppCore.Core.Entities
{
    public class Customer : EntityRootBase
    {
        private readonly List<CreditCard> _creditCards = new List<CreditCard>();

        public string FirstName { get; protected set; } = default(string)!;
        public string LastName { get; protected set; } = default(string)!;
        public string Email { get; protected set; } = default(string)!;
        public decimal Balance { get; protected set; }
        public Guid CountryId { get; protected set; }

        public virtual IEnumerable<CreditCard> CreditCards => _creditCards.AsReadOnly();

        public virtual void ChangeEmail(string email)
        {
            if (Email != email)
            {
                Email = email;

                //DomainEvents.Raise<CustomerChangedEmail>(new CustomerChangedEmail() { Customer = this });
            }
        }

        public virtual void AddAmount(decimal amount)
        {
            // Check available cards
            if (GetCreditCardsAvailable().Any())
            {
                //DomainEvents.Raise<CustomerChangedEmail>(new CustomerChangedEmail() { Customer = this });
            }
        }

        public static Customer Create(string firstname, string lastname, string email, Guid countryId)
        {
            return Create(Guid.NewGuid(), firstname, lastname, email, countryId);
        }

        public static Customer Create(
            Guid id,
            string firstname,
            string lastname,
            string email,
            Guid countryId)
        {
            if (string.IsNullOrEmpty(firstname))
            {
                throw new ArgumentNullException("firstname");
            }

            if (string.IsNullOrEmpty(lastname))
            {
                throw new ArgumentNullException("lastname");
            }

            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException("email");
            }

            if (countryId == Guid.Empty)
            {
                throw new ArgumentNullException("country");
            }

            Customer customer = new Customer
            {
                Id = id,
                FirstName = firstname,
                LastName = lastname,
                Email = email,
                CountryId = countryId
            };

            customer.AddDomainEvent(new CustomerCreatedIntegrationEvent());
            //DomainEvents.Raise<CustomerCreated>(new CustomerCreated() { Customer = customer });

            return customer;
        }

        public virtual ReadOnlyCollection<CreditCard> GetCreditCardsAvailable()
        {
            return _creditCards.FindAll(new CreditCardAvailableSpec(DateTime.Today).IsSatisfiedBy)
                .AsReadOnly();
        }

        public virtual void Add(CreditCard creditCard)
        {
            _creditCards.Add(creditCard);

            //DomainEvents.Raise<CreditCardAdded>(new CreditCardAdded() { CreditCard = creditCard });
        }
    }
}
