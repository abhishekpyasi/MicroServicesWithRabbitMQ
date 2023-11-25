using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Play.Common;
using Play.Inventory.Service.Clients;
using Play.Inventory.Service.Entities;

namespace Play.Inventory.Service.Controllers
{
    [ApiController]
    [Route("Items")]
    public class ItemsController : ControllerBase
    {
        private const string AdminRole = "Admin";

        private readonly IRepository<InventoryItem> inventoryItemsRepository;
        private readonly IRepository<CatalogItem> catalogItemsRepository;

        public ItemsController(IRepository<InventoryItem> inventoryItemsRepository, IRepository<CatalogItem> catalogItemsRepository)
        {
            this.inventoryItemsRepository = inventoryItemsRepository;
            this.catalogItemsRepository = catalogItemsRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventoryItemDto>>> GetAsync(Guid userID)
        {

            if (userID == Guid.Empty)
            {
                return BadRequest();
            }

            //check userid for access token

            var currentUserID = User.FindFirstValue("sub");

            if (Guid.Parse(currentUserID) != userID)
            {
                if (!User.IsInRole(AdminRole))

                {
                    return Unauthorized();

                }

            }


            var inventoryItemEntities = await inventoryItemsRepository.GetAllAsync(item => item.UserId == userID);
            var itemIds = inventoryItemEntities.Select(item => item.CatalogItemID);
            var catalogItemEntities = await catalogItemsRepository.GetAllAsync(item => itemIds.Contains(item.Id));

            var inventoryItemDtos = inventoryItemEntities.Select(inventoryItem =>
            {
                var catalogItem = catalogItemEntities.Single(catalogItem => catalogItem.Id == inventoryItem.CatalogItemID);
                return inventoryItem.AsDto(catalogItem.Name, catalogItem.Description);
            });

            return Ok(inventoryItemDtos);


        }
        [HttpPost]
        [Authorize(Roles = AdminRole)]
        public async Task<ActionResult> PostAsync(GrantItemsDto grantItemsDto)
        {



            var inventoryItem = await inventoryItemsRepository.GetItemAsync(item => item.UserId == grantItemsDto.UserId

            && item.CatalogItemID == grantItemsDto.CatalogItemID);

            if (inventoryItem == null)
            {

                inventoryItem = new InventoryItem
                {

                    CatalogItemID = grantItemsDto.CatalogItemID,
                    UserId = grantItemsDto.UserId,
                    Quantity = grantItemsDto.Quantity,
                    AcquiredDate = DateTimeOffset.UtcNow,
                };

                await inventoryItemsRepository.CreateAsync(inventoryItem);

            }

            else
            {

                inventoryItem.Quantity = inventoryItem.Quantity + grantItemsDto.Quantity;
                await inventoryItemsRepository.UpdateAsync(inventoryItem);
            }
            return Ok();
        }



    }
}