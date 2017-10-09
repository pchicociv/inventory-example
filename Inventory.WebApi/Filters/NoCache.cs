using System;
using System.Web.Http.Filters;

namespace Inventory.WebApi.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class NoCacheAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            actionExecutedContext.Response.Headers.CacheControl = new System.Net.Http.Headers.CacheControlHeaderValue()
            {
                NoCache = true,
                MustRevalidate = true,
                MaxAge = new TimeSpan(0, 0, 0),
                NoStore = true
            };
            base.OnActionExecuted(actionExecutedContext);
        }
    }
}
