using Newtonsoft.Json;
using System.Text;

namespace Inventory.Models.RabbitMQ
{
    public static class DomainEventExtensions
    {
        public static byte[] ToMessageBody<T>(this T e) where T : DomainEvent
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(e));
        }

        public static T ToDomainEvent<T>(this byte[] body) where T : DomainEvent
        {
            string jsonified = Encoding.UTF8.GetString(body);
            T item = JsonConvert.DeserializeObject<T>(jsonified, new JsonSerializerSettings
            {
                ContractResolver = new DomainEventContractResolver()
            });

            return item;
        }

    }
}
