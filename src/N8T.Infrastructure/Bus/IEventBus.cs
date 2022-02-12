using System.Threading;
using System.Threading.Tasks;
using N8T.Core.Domain;

namespace N8T.Infrastructure.Bus
{
    public interface IEventBus
    {
        Task PublishAsync<TEvent>(TEvent @event, string[] topics = default(string[]),
            CancellationToken token = default(CancellationToken))
            where TEvent : IDomainEvent;

        Task SubscribeAsync<TEvent>(string[] topics = default(string[]),
            CancellationToken token = default(CancellationToken))
            where TEvent : IDomainEvent;
    }
}
