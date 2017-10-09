using Inventory.Models.Repositories;
using Inventory.Models.Security;
using System.Collections.Generic;

namespace Inventory.Repositories
{
    //We would use this repository to manage ApiUsers
    public class ApiUsersRepository : IApiUsersRepository
    {
        public IEnumerable<ApiUser> GetAll()
        {
            return new List<ApiUser>
            {
                new ApiUser
                {

                }
            };
        }

        public ApiUser GetByApiKey(string apiKey)
        {
            return new ApiUser();
        }
    }
}
