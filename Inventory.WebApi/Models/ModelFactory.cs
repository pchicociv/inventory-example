using Inventory.Models.Core;
using System;
using System.Net.Http;
using System.Web.Http.Routing;

namespace Inventory.WebApi.Models
{
    public class ModelFactory
    {
        UrlHelper _urlHelper;

        public ModelFactory(HttpRequestMessage request)
        {
            _urlHelper = new UrlHelper(request);
        }

        public ItemModel Create(Item item)
        {
            return new ItemModel
            {
                Id = item.Id,
                ExpirationDate = item.Expiration.ToString("dd/MM/yyyy"),
                ItemType = item.ItemType,
                Label = item.Label,
                //Is a good idea to provide the clients with the url where they can found the newly inserted resource
                Url = _urlHelper.Link("Inventory", new { label = item.Label })
            };
        }
        public Item Parse(ItemModel itemModel)
        {
            try
            {
                var item = new Item();

                item.Label = itemModel.Label;
                item.ItemType = itemModel.ItemType;
                //Dates are complex. In a globally distributed application with clients around the world,
                // time zones must be taken into consideration. A good approach is choosing a zone as local
                // and convert every date from and to that zone.
                item.Expiration = DateTime.ParseExact(itemModel.ExpirationDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);

                return item;
            }
            catch (Exception)
            {

                throw;
            }
            
        }
    }
}