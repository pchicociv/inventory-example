using System;
using System.Globalization;
using System.Text;
using Inventory.Models.DomainEvents;

namespace Inventory.WebFx
{
    public delegate void ContextDisposingEventHandler(CallContext context, EventArgs e);
    public delegate void ContextCreatedEventHandler(CallContext context, EventArgs e);

    public class CallContext : ICallContext, IDisposable
    {
        public static event ContextDisposingEventHandler ContextDisposing;
        private static void OnContextDisposing(CallContext sender)
        {
            if (ContextDisposing != null)
                ContextDisposing(sender, new EventArgs());
        }

        public static event ContextCreatedEventHandler ContextCreated;
        private static void OnContextCreated(CallContext sender)
        {
            if (ContextCreated != null)
                ContextCreated(sender, new EventArgs());
        }

        //Setters must be public for json deserialization

        public Guid CallId { get; set; }

        public string FullCallId
        {
            get { return (Parent != null ? Parent.FullCallId + "->" : "") + CallId.ToString(); }
        }

        public CultureInfo CultureInfo { get; set; }

        public String Description { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public CallContext Parent { get; set; }

        public CallContext()
        {

        }
        public CallContext(string description)
        {
            CallId = Guid.NewGuid();
            StartTime = DateTime.Now;
            Description = description;
            OnContextCreated(this);
        }
        private CallContext(Guid callId)
        {
            CallId = callId;
            StartTime = DateTime.Now;
            OnContextCreated(this);
        }
        public CallContext(Guid parentCallId, string description)
        {
            CallId = Guid.NewGuid();
            StartTime = DateTime.Now;
            Description = description;
            Parent = new CallContext(parentCallId);
            OnContextCreated(this);
        }

        public CallContext(CallContext parentContext, string contextDescription)
        {
            CallId = Guid.NewGuid();
            StartTime = DateTime.Now;
            Description = parentContext.Description + "->" + contextDescription;
            Parent = parentContext;
            OnContextCreated(this);
        }
        public void Dispose()
        {
            EndTime = DateTime.Now;
            OnContextDisposing(this);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(this.FullCallId);
            sb.Append(" ");
            sb.Append(this.StartTime.ToString("dd/MM/yyyy HH:mm:ss.fff"));
            sb.Append(" ");
            sb.Append(this.EndTime.ToString("dd/MM/yyyy HH:mm:ss.fff"));
            sb.Append(" ");
            sb.Append(this.Description);
            return sb.ToString();
        }
    }


}
