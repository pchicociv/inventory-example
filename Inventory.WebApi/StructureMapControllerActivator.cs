using StructureMap;
using System;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;

namespace Inventory.WebApi
{
    public class StructureMapControllerActivator : IHttpControllerActivator
    {
        private static IContainer _container;

        public StructureMapControllerActivator(IContainer container)
        {
            if (container == null) throw new ArgumentNullException("container");
            _container = container;
        }

        public IHttpController Create(HttpRequestMessage request, HttpControllerDescriptor controllerDescriptor, Type controllerType)
        {
            try
            {
                var scopedContainer = _container.GetNestedContainer();
                scopedContainer.Inject(typeof(HttpRequestMessage), request);
                request.RegisterForDispose(scopedContainer);
                return (IHttpController)scopedContainer.GetInstance(controllerType);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static IContainer CurrentContainer()
        {
            return _container;
        }
    }
}