using Inventory.Models;
using Inventory.Models.Repositories;
using Inventory.Models.Security;
using Inventory.WebApi.Models;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Inventory.WebApi.Controllers
{
    //This could be a good entry point for api token management
    public class TokenController : BaseApiController
    {
        IApiUsersRepository _apiUsersRepository;
        public TokenController(IItemsRepository itemsRepository, IApiUsersRepository apiUsersRepository) : base(itemsRepository)
        {
            _apiUsersRepository = apiUsersRepository;
        }

        // Users can request a token here using their appKey. Note this won't work as routing for this controller is not cofigured
        public IHttpActionResult Post([FromBody]TokenRequestModel model)
        {
            try
            {
                var user = _apiUsersRepository.GetByApiKey(model.ApiKey);
                if (user != null)
                {
                    var secret = user.Secret;

                    if (ComputeSignature(secret,model.Signature))
                    {
                        var authToken = BuildAuthToken(user);
                        // at this point we shoud store the token

                        // we should not send back this authToken as is. 
                        //  Instead we should send only the token value and expiration information
                        return Ok(authToken);
                    }
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
             
            }
            return BadRequest();
        }

        private AuthToken BuildAuthToken(ApiUser user)
        {
            return new AuthToken()
            {
                ApiUser = user,
                Expiration = DateTime.UtcNow.AddDays(7),
                Id = 1,
                Token = "F46B9A9B642943C182E319880F85A554"
            };
        }

        private bool ComputeSignature(string secret, string signature)
        {
            //TODO: Implement a cryptographic function to validate signature
            return true;
        }
    }
}