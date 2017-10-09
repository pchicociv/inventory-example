using Hangfire;
using Hangfire.MemoryStorage;
using Inventory.Models.DomainEvents;
using Inventory.Models.RabbitMQ;
using Inventory.WebFx;
using Microsoft.Owin;
using Owin;
using System;

[assembly: OwinStartup(typeof(Inventory.BackgroundWorker.Startup))]
namespace Inventory.BackgroundWorker
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            GlobalConfiguration.Configuration.UseMemoryStorage();

            //Initialize Hangfire
            app.UseHangfireDashboard();
            app.UseHangfireServer();

            //Job should execute everyday at one minute to local midnight
            //RecurringJob.AddOrUpdate(() =>  FinishDay() , "23 59 * * *");

            //But for demo purposes we set it to run every minute
            RecurringJob.AddOrUpdate(() => FinishDay(), "* * * * *");

            //Also for this demo, lets run it a first time in 5 seconds so the tests dont have to wait that long
            BackgroundJob.Schedule(() => FinishDay(),TimeSpan.FromSeconds(5));
        }

        public static void FinishDay()
        {
            DayFinished e = new DayFinished(new CallContext("Day Finished Task from background worker"),DateTime.Today);
            RabbitMqNotificationBus bus = new RabbitMqNotificationBus();
            bus.Publish(e);
        }
    }
}
