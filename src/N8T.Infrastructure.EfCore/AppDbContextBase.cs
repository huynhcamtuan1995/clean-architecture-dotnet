using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using N8T.Core.Domain;

namespace N8T.Infrastructure.EfCore
{
    public abstract class AppDbContextBase : DbContext, IDomainEventContext, IDbFacadeResolver
    {
        protected AppDbContextBase(DbContextOptions options) : base(options)
        {
        }

        public IEnumerable<EventBase> GetDomainEvents()
        {
            List<EntityEntry<EntityRootBase>> domainEntities = ChangeTracker
                .Entries<EntityRootBase>()
                .Where(x =>
                    x.Entity.DomainEvents != null &&
                    x.Entity.DomainEvents.Any())
                .ToList();

            List<EventBase> domainEvents = domainEntities
                .SelectMany(x => x.Entity.DomainEvents)
                .ToList();

            domainEntities.ForEach(entity => entity.Entity.DomainEvents.Clear());

            return domainEvents;
        }
    }
}
