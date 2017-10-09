namespace Inventory.Models.DomainEvents
{
    public class SystemEvent : DomainEvent
    {
        public SystemEvent()
        {

        }
        public SystemEvent(ICallContext callContext) : base(callContext)
        {
        }
    }
}
