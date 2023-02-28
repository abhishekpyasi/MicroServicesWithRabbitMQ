using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Play.Catlog.Service.DTO;
using Play.Catlog.Service.Entities;
using Play.Common;

namespace Play.Catlog.Service.Controllers
{
    [ApiController]
    [Route("Items")]
    public class ItemsController : ControllerBase
    {
        private readonly IRepository<Item> itemsRepository;

        public ItemsController(IRepository<Item> itemsRepository)
        {
            this.itemsRepository = itemsRepository;
        }

        [HttpGet]
        public async Task<ActionResult<ItemDto>> GetAsync()
        {


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
            // As the name suggests, this method allows us 
            // to set Location URI of the newly created resource by 
            // specifying the name of an action where we can retrieve our resource.

            /* actionName - by default it is controller action method name but can also be assigned using [ActionName("...")] attribute
        controllerName - name of the controller where our action resides
        routeValues - info necessary to generate a correct URL, for example, path or query parameters here its ID
        value - content to return in a response body  This is sent in response header location attribute*/


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
            return NoContent();
        }

    }
}