using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Models.DomainEvents
{
   public interface ICallContext
    {
        Guid CallId { get; }
        string FullCallId { get;  }
    }
}
