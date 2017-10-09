using Inventory.Models.Repositories;
using Inventory.WebApi.Models;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Inventory.WebApi.Controllers
{
    public abstract class BaseApiController : ApiController
    {
        public BaseApiController(IItemsRepository itemsRepository)
        {
            _itemsRepository = itemsRepository;
        }

        protected IItemsRepository ItemsRepository
        {
            get { return _itemsRepository; }
        }
        private IItemsRepository _itemsRepository;

        protected ModelFactory ModelFactory
        {
            get
            {
                if (_modelFactory == null)
                {
                    _modelFactory = new ModelFactory(this.Request);
                }
                return _modelFactory;
            }
        }
        private ModelFactory _modelFactory;

        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
        }
    }
}
