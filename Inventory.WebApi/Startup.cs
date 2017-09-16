using Inventory.WebFx;
using Microsoft.Owin;
using Owin;
using Serilog;
using StructureMap;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Dispatcher;

[assembly: OwinStartup(typeof(Inventory.WebApi.Startup))]
namespace Inventory.WebApi
{
    public class Startup
    {

        public void Configuration(IAppBuilder appBuilder)
        {
            HttpConfiguration httpConfiguration = new HttpConfiguration();
            WebApiConfig.Register(httpConfiguration);
            appBuilder.UseWebApi(httpConfiguration);

            Log.Logger = new LoggerConfiguration()
               .ReadFrom.AppSettings()
               .CreateLogger();

            CallContext.ContextCreated += CallContext_ContextCreated;
            CallContext.ContextDisposing += CallContext_ContextDisposing;

            var container = InitializeIoC(httpConfiguration);

            NotificationConsumer.Subscribe();

            var context = new OwinContext(appBuilder.Properties);
            var token = context.Get<CancellationToken>("host.OnAppDisposing");
            if (token != CancellationToken.None)
            {
                token.Register(() =>
                {
                    NotificationConsumer.Dispose();
                });
            }
        }

        //Subscription to CallContext events should be placed outside Startup.
        // This class is already doing so much!
        private void CallContext_ContextDisposing(CallContext context, System.EventArgs e)
        {
            Log.Information("Start context: " + context.ToString());
        }

        private void CallContext_ContextCreated(CallContext context, System.EventArgs e)
        {
            Log.Information("End context: " + context.ToString());
        }

        public Container InitializeIoC(HttpConfiguration config)
        {
            //this configuration would be better in a different file, even in a different project
            Container container = new Container();
            config.Services.Replace(typeof(IHttpControllerActivator), new StructureMapControllerActivator(container));

            container.Configure(x => x.For<Inventory.Models.Repositories.IItemsRepository>().Use<Inventory.Repositories.ItemsRepository>());
            container.Configure(x => x.For<Inventory.Models.DomainEvents.IEventStore>().Use<Inventory.Models.RabbitMQ.RabbitMqNotificationBus>());
            container.Configure(x => x.For<Inventory.Models.DomainEvents.ICallContext>().Use<CallContext>());

            return container;
        }
    }
}