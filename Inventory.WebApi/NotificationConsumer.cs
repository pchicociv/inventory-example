using Inventory.Models.DomainEvents;
using Inventory.Models.RabbitMQ;
using Inventory.WebFx;
using System;

namespace Inventory.WebApi
{
    public static class NotificationConsumer
    {
        static NotificationConsumer()
        {
            _bus = new RabbitMqNotificationBus();
        }

        static RabbitMqNotificationBus _bus;

        public static void Subscribe()
        {
            _bus.Subscribe<DayFinished>(e => { CheckExpiredItems(e); });
        }

        public static void CheckExpiredItems(DayFinished e)
        {
            foreach (var item in Inventory.Models.Core.Inventory.Instance.GetAll())
            {
                DateTime finishedDay = DateTime.ParseExact(e.Date, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);

                var itemExpiredEvent = item.CheckExpiration(finishedDay, new CallContext());
                if (itemExpiredEvent != null)
                    _bus.Publish(itemExpiredEvent);

            }
        }

        public static void Dispose()
        {
            _bus.Dispose();
        }
    }
}