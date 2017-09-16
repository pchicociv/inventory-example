using Inventory.Models.DomainEvents;
using Inventory.Models.Repositories;
using Inventory.WebApi.Filters;
using Inventory.WebApi.Models;
using Inventory.WebFx;
using Serilog;
using StructureMap;
using System;
using System.Linq;
using System.Web.Http;

namespace Inventory.WebApi.Controllers
{
    [IncludeCallContext]
    [NoCache]
    [InventoryAuthorize]
    public class InventoryController : BaseApiController
    {
        public InventoryController(IItemsRepository itemsRepository)
            : base(itemsRepository) { }

        public IHttpActionResult Get()
        {
            object callContextObj = null;
            if (Request.Properties.TryGetValue("CallContext", out callContextObj))
            {
                CallContext callContext = (CallContext)callContextObj;
            }

            return Ok(ItemsRepository.GetAll().Select(i => ModelFactory.Create(i)));
        }

        public IHttpActionResult Get(string label)
        {
            var item = ItemsRepository.GetByLabel(label);
            if (item == null)
            {
                return NotFound();
            }
            return Ok(ModelFactory.Create(item));
        }

        public IHttpActionResult Post([FromBody]ItemModel model)
        {
            try
            {
                ICallContext callContext = GetCallContext();

                Log.Information("Adding item");
                var newItem = ModelFactory.Parse(model);
                newItem = ItemsRepository.Save(callContext, newItem);
                var newItemModel = ModelFactory.Create(newItem);
                Log.Information("Item added");
                return Created(newItemModel.Url, newItemModel);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        private ICallContext GetCallContext()
        {
            object callContextObj = null;
            ICallContext callContext;
            if (Request.Properties.TryGetValue("CallContext", out callContextObj))
            {
                callContext = (ICallContext)callContextObj;
            }
            else
            {
                IContainer container = StructureMapControllerActivator.CurrentContainer();
                callContext = container.GetInstance<ICallContext>();
            }

            return callContext;
        }

        public IHttpActionResult Delete(string label)
        {
            try
            {
                ICallContext callContext = GetCallContext();

                var itemToDelete = ItemsRepository.GetByLabel(label);
                if (itemToDelete == null)
                {
                    return NotFound();
                }

                itemToDelete = ItemsRepository.Delete(callContext, itemToDelete);
                if (itemToDelete == null)
                {
                    return BadRequest();
                }

                return Ok(ModelFactory.Create(itemToDelete));

            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
