using Inventory.Models.DomainEvents;
using Inventory.Models.RabbitMQ;
using System;

namespace Inventory.Consumer
{
    class Program
    {
        static void Main(string[] args)
        {
            RabbitMqNotificationBus bus = new RabbitMqNotificationBus();

            bus.Subscribe<ItemCreated>(a => 
            {
                Console.WriteLine(a.ToString());
            });
            bus.Subscribe<ItemExpired>(a =>
            {
                Console.WriteLine(a.ToString());
            });
            bus.Subscribe<ItemTaken>(a =>
            {
                Console.WriteLine(a.ToString());
            });
            bus.Subscribe<DayFinished>(a =>
            {
                Console.WriteLine(a.ToString());
            });
            Console.ReadKey();
        }
    }
}
