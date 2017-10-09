using Inventory.Models.DomainEvents;
using Inventory.WebFx;
using Newtonsoft.Json.Serialization;
using System;

namespace Inventory.Models.RabbitMQ
{
    //This class is necessary to deserialize json CallContexts inside DomainEvents
    public class DomainEventContractResolver : DefaultContractResolver
    {
        protected override JsonObjectContract CreateObjectContract(Type objectType)
        {
            if (objectType == typeof(ICallContext))
            {
               return base.CreateObjectContract(typeof(CallContext));
            }

            return base.CreateObjectContract(objectType);
        }
    }
}
