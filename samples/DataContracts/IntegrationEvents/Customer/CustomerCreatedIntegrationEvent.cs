using N8T.Core.Domain;

namespace IntegrationEvents.Customer
{
    [DaprPubSubName(PubSubName = "pubsub")]
    public class CustomerCreatedIntegrationEvent : EventBase
    {
        public override void Flatten()
        {
        }
    }
}
