using System;
using System.Net;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Net.Http;
using System.Text;
using System.Security.Principal;
using System.Threading;
using Inventory.Repositories;

namespace Inventory.WebApi.Filters
{
    public class InventoryAuthorizeAttribute : AuthorizationFilterAttribute
    {
        private bool _perUser;
        public InventoryAuthorizeAttribute(bool perUser = true)
        {
            _perUser = perUser;
            _authRepository = new AuthRepository();
        }

        private AuthRepository _authRepository { get; set; }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            const string APIKEYNAME = "apikey";
            const string TOKENNAME = "token";

            var query = HttpUtility.ParseQueryString(actionContext.Request.RequestUri.Query);

            if (!string.IsNullOrWhiteSpace(query[APIKEYNAME]) &&
              !string.IsNullOrWhiteSpace(query[TOKENNAME]))
            {
                var apikey = query[APIKEYNAME];
                var token = query[TOKENNAME];

                var authToken = _authRepository.GetAuthToken(token);

                if (authToken != null && authToken.ApiUser.AppId == apikey && authToken.Expiration > DateTime.UtcNow)
                {
                    if (_perUser)
                    {
                        if (Thread.CurrentPrincipal.Identity.IsAuthenticated)
                        {
                            return;
                        }

                        var authHeader = actionContext.Request.Headers.Authorization;

                        if (authHeader != null)
                        {
                            if (authHeader.Scheme.Equals("basic", StringComparison.OrdinalIgnoreCase) &&
                              !string.IsNullOrWhiteSpace(authHeader.Parameter))
                            {
                                var rawCredentials = authHeader.Parameter;
                                var encoding = Encoding.GetEncoding("iso-8859-1");
                                var credentials = encoding.GetString(Convert.FromBase64String(rawCredentials));
                                var split = credentials.Split(':');
                                var username = split[0];
                                var password = split[1];

                                //TODO: Validate user and password
                                if (true)
                                {
                                    var principal = new GenericPrincipal(new GenericIdentity(username), null);
                                    Thread.CurrentPrincipal = principal;
                                    return;
                                }
                            }
                        }
                    }
                    else
                    {
                        return;
                    }
                }
            }

            HandleUnauthorized(actionContext);

        }

        void HandleUnauthorized(HttpActionContext actionContext)
        {
            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
            if (_perUser)
            {
                //We have not implemented a login page so the location won't have any effect.
                //Some browsers may accept the WWW-Authenticate header and prompt the user for username and password
                actionContext.Response.Headers.Add("WWW-Authenticate",
                  "Basic Scheme='Inventory' location='http://localhost/account/login'");
            }
        }
    }
}