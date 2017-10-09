namespace Inventory.Models.DomainEvents
{
    public class ItemCreated : UserEvent
    {
        public ItemCreated()
        {

        }
        public ItemCreated(ICallContext callContext) : base(callContext)
        {
        }
    }
}
