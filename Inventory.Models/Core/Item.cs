using Inventory.Models.DomainEvents;
using System;

namespace Inventory.Models.Core
{
    public class Item
    {
        public Item()
        {
            Id = -1;
            Expiration = DateTime.Today.AddDays(30);
            Label = "Unlabeled item";
            ItemType = "UNKNOWN";
        }
        public long Id { get; set; }
        public string Label { get; set; }
        public string ItemType { get; set; }

        public ItemTaken Delete(ICallContext callContext)
        {
            return new ItemTaken(callContext, Label);
        }

        public DateTime Expiration { get; set; }

        public ItemCreated Create(ICallContext callContext)
        {
            return new ItemCreated(callContext);
        }

        public ItemExpired CheckExpiration(DateTime date, ICallContext callContext)
        {
            if (this.Expiration < date)
            {
                return new ItemExpired(callContext, Label, Expiration);
            }
            return null;
        }
    }
}
