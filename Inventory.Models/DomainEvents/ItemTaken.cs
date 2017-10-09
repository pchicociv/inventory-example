namespace Inventory.Models.DomainEvents
{
    public class ItemTaken : UserEvent
    {
        public ItemTaken()
        {

        }
        public ItemTaken(ICallContext callContext) : base(callContext)
        {

        }
        public ItemTaken(string label)
        {
            Label = label;
        }
        public ItemTaken(ICallContext callContext,string label) : base(callContext)
        {
            Label = label;
        }
        public string Label { get; set; }
    }
}
