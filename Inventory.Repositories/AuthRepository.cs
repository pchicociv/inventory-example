using Inventory.Models.Security;
using System;

namespace Inventory.Repositories
{
    public class AuthRepository
    {
        public AuthToken GetAuthToken(string token)
        {
            return new AuthToken()
            {
                ApiUser = new ApiUser()
                {
                    AppId = "88DA3BBAC33F4C35A7B6453185F38BB2",
                    Secret = "EC1B3C3184C047F381B2A13971064777"
                },
                Token = token,
                Expiration = DateTime.Now.AddDays(30),
                Id = 1
            };
        }
    }
}
