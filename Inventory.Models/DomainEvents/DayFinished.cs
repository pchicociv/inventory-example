using System;

namespace Inventory.Models.DomainEvents
{
    public class DayFinished : SystemEvent
    {
        public DayFinished()
        {

        }
        public DayFinished(DateTime date)
        {
            Date = date.Date.ToLocalTime().ToString("yyyyMMdd");
        }

        public DayFinished(ICallContext callContext) : base(callContext)
        {

        }
        public DayFinished(ICallContext callContext, DateTime date) : base(callContext)
        {
            Date = date.Date.ToLocalTime().ToString("yyyyMMdd");
        }

        public string Date { get; set; }
    }
}
