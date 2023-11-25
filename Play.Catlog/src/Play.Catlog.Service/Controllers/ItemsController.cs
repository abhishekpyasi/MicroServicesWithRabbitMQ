using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Play.Catlog.Service.DTO;
using Play.Catlog.Service.Entities;
using Play.Common;
using static Play.Catalog.Contract.Contracts;

namespace Play.Catlog.Service.Controllers
{
    [ApiController]
    [Route("Items")]
    [Authorize(Roles = AdminRole)]
    public class ItemsController : ControllerBase
    {
        private const string AdminRole = "Admin";
        private readonly IRepository<Item> itemsRepository;

        private readonly IPublishEndpoint publishEndpoint;

        public ItemsController(IRepository<Item> itemsRepository, IPublishEndpoint publishEndpoint)
        {
            this.itemsRepository = itemsRepository;
            this.publishEndpoint = publishEndpoint;
        }

        [HttpGet]
        public async Task<ActionResult<ItemDto>> GetAsync()
        {
            await Task.Delay(5000);

            var items = (IEnumerable<ItemDto>)(await itemsRepository.GetAllAsync()).Select(item => item.AsDto());

            return Ok(items);
        }

        // GET /items/{id}
        [HttpGet("{id}")]

        //ActionResult return type give us the flexibility of returning to types of return types 
        public async Task<ActionResult<ItemDto>> GetByIdAsync(Guid id)
        {
            var item = await itemsRepository.GetItemAsync(id);

            if (item == null)
            {
                return NotFound();
            }
            return Ok(item.AsDto());
        }

        // POST /items
        [HttpPost]
        public async Task<ActionResult<ItemDto>> PostAsync(CreateItemDto createItemDto)
        {
            var item = new Item(Guid.NewGuid(),
             createItemDto.Name, createItemDto.Description,
             createItemDto.Price, DateTimeOffset.UtcNow);
            await itemsRepository.CreateAsync(item);
            // As the name s    uggests, this method allows us 
            // to set Location URI of the newly created resource by 
            // specifying the name of an action where we can retrieve our resource.

            /* actionName - by default it is controller action method name but can also be assigned using [ActionName("...")] attribute
        controllerName - name of the controller where our action resides
        routeValues - info necessary to generate a correct URL, for example, path or query parameters here its ID
        value - content to return in a response body  This is sent in response header location attribute*/

            //publish in end point

            await publishEndpoint.Publish(new CatalogItemCreated(item.Id, item.Name, item.Description));
            return CreatedAtAction(nameof(GetByIdAsync), new { id = item.Id }, item);



        }

        // PUT /items/{id}

        /* for put operation do not return any item */
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync(Guid id, UpdateItemDto updateItemDto)
        {
            var existingitem = await itemsRepository.GetItemAsync(id);
            if (existingitem == null)
            {

                return NotFound();
            }

            existingitem.Name = updateItemDto.Name;
            existingitem.Description = updateItemDto.Description;
            existingitem.price = updateItemDto.Price;

            await itemsRepository.UpdateAsync(existingitem);

            await publishEndpoint.Publish(new CatalogItemUpdated(existingitem.Id, existingitem.Name, existingitem.Description));




            return NoContent();
        }

        // DELETE /items/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var item = await itemsRepository.GetItemAsync(id);

            if (item == null)

            {

                return NotFound();
            }
            await itemsRepository.RemoveAsync(item.Id);
            await publishEndpoint.Publish(new CatalogItemDeleted(item.Id));

            return NoContent();
        }

    }
}