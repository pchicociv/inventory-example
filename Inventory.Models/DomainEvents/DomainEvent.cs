using Inventory.Models.DomainEvents;
using System;

namespace Inventory.Models
{
    public abstract class DomainEvent
    {
        public DateTime OccurredOn { get; set; }
        public ICallContext CallContext { get; set; }
        public DomainEvent()
        {

        }
        public DomainEvent(ICallContext callContext)
        {
            OccurredOn = DateTime.Now;
            CallContext = callContext;
        }

        public override string ToString()
        {
            return this.GetType().Name + " event occurred at " + OccurredOn.ToString("HH:mm:ss.fff") + " in call context " + CallContext.FullCallId;
        }
    }

}
