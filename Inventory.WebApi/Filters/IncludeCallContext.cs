using Inventory.WebFx;
using System.Collections.Generic;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Inventory.WebApi.Filters
{
    public class IncludeCallContextAttribute : ActionFilterAttribute
    {
        CallContext _callContext;

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            _callContext = new CallContext(actionContext.Request.RequestUri.PathAndQuery);
            actionContext.Request.Properties.Add(new KeyValuePair<string, object>("CallContext", _callContext));
            base.OnActionExecuting(actionContext);
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            _callContext.Dispose();
            base.OnActionExecuted(actionExecutedContext);
        }

    }
}
