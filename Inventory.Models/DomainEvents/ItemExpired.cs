using System;

namespace Inventory.Models.DomainEvents
{
    public class ItemExpired : SystemEvent
    {
        public ItemExpired()
        {

        }
        public ItemExpired(ICallContext callContext) : base(callContext)
        {

        }
        public ItemExpired(string label, DateTime expirationDate)
        {
            Label = label;
            ExpirationDate = expirationDate;
        }
        public ItemExpired(ICallContext callContext,string label, DateTime expirationDate) : base(callContext)
        {
            Label = label;
            ExpirationDate = expirationDate;
        }
        public string Label { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}
