using System;

namespace InsureFlow.PropostaService.Application.Ports
{
    public interface IEventPublisher
    {
        void Publish(string exchange, string routingKey, object @event);
    }
}
