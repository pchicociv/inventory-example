using Inventory.Models.Core;
using Inventory.Models.DomainEvents;
using Inventory.Models.Repositories;
using System.Collections.Generic;

namespace Inventory.Repositories
{
    public class ItemsRepository : IItemsRepository
    {
        IEventStore _eventStore;
        public ItemsRepository(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }

        public Item Delete(ICallContext callContext,Item item)
        {
            Item deletedItem = Models.Core.Inventory.Instance.Remove(item);
            if (deletedItem != null)
            {
                var e = item.Delete(callContext);
                _eventStore.Publish(e);
            }
            return deletedItem;
        }

        public IEnumerable<Item> GetAll()
        {
            return Models.Core.Inventory.Instance.GetAll();
        }

        public Item GetById(long id)
        {
            return Models.Core.Inventory.Instance.Get(id);
        }

        public Item GetByLabel(string label)
        {
            return Models.Core.Inventory.Instance.Get(label);
        }

        public Item Save(ICallContext callContext, Item newItem)
        {
            newItem = Models.Core.Inventory.Instance.Add(newItem);
            var e = newItem.Create(callContext);
            //If Inventory.Consumer is not running, no one is consuming ItemCreated events! They will be kept in the queue permanently
            _eventStore.Publish(e);
            return newItem;
        }
    }
}
