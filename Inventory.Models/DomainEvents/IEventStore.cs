using System;
using System.Threading.Tasks;

namespace Inventory.Models.DomainEvents
{
    public interface IEventStore
    {
        void Publish<T>(T e) where T:DomainEvent;

        void Subscribe<T>(Action<T> callback) where T : DomainEvent;

        Task SubscribeAndWaitFirstMessage<T>(Action<T> callback) where T : DomainEvent;
    }
}
