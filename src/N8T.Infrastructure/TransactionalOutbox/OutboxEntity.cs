using System;
using System.Reflection;
using System.Text.Json.Serialization;
using N8T.Core.Domain;
using Newtonsoft.Json;

namespace N8T.Infrastructure.TransactionalOutbox
{
    public class OutboxEntity
    {
        public OutboxEntity()
        {
            // only for SimpleSystem.Text.Json to deserialized data
        }

        public OutboxEntity(Guid id, DateTime occurredOn, IDomainEvent @event)
        {
            Id = id.Equals(Guid.Empty) ? Guid.NewGuid() : id;
            OccurredOn = occurredOn;
            Type = @event.GetType().FullName;
            Data = JsonConvert.SerializeObject(@event);
        }

        [JsonInclude] public Guid Id { get; private set; }

        [JsonInclude] public DateTime OccurredOn { get; private set; }

        [JsonInclude] public string Type { get; private set; }

        [JsonInclude] public string Data { get; private set; }

        public virtual IDomainEvent RecreateMessage(Assembly assembly)
        {
            return (IDomainEvent)JsonConvert.DeserializeObject(Data, assembly.GetType(Type)!);
        }
    }
}
