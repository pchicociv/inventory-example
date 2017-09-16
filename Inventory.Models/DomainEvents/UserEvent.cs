using System.Threading;

namespace Inventory.Models.DomainEvents
{
    public abstract class UserEvent: DomainEvent
    {
        public UserEvent()
        {

        }
        public UserEvent(ICallContext callContext) : base(callContext)
        {
            User = Thread.CurrentPrincipal.Identity.Name;
        }
        public string User { get; private set; }
    }
}
