using Inventory.Models.Security;
using System.Collections.Generic;

namespace Inventory.Models.Repositories
{
    public interface IApiUsersRepository
    {
        IEnumerable<ApiUser> GetAll();
        ApiUser GetByApiKey(string apiKey);
    }
}
