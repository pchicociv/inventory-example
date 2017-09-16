using Inventory.Models.Core;
using Inventory.Models.DomainEvents;
using System.Collections.Generic;

namespace Inventory.Models.Repositories
{
    public interface IItemsRepository
    {
        IEnumerable<Item> GetAll();
        Item GetById(long id);
        Item Save(ICallContext callContext, Item newItem);
        Item Delete(ICallContext callContext, Item item);
        Item GetByLabel(string label);
    }
}
