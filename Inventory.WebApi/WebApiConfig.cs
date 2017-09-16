using Inventory.WebApi.Filters;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Inventory.WebApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {

            //SSL Configuration
            config.Filters.Add(new RequireHttpsAttribute());

            //CORS Configuration
            EnableCorsAttribute cors = new EnableCorsAttribute("*", "*", "GET,POST,DELETE");
            config.EnableCors(cors);

            //Route Configuration
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "Inventory",
                routeTemplate: "api/inventory/{label}",
                defaults: new { controller = "inventory", label = RouteParameter.Optional }
            );


        }
    }
}
