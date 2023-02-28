using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Play.Inventory.Service.Entities;

namespace Play.Inventory.Service
{
    public static class Extensions
    {
        public static InventoryItemDto AsDto(this InventoryItem item)

        {

            return new InventoryItemDto(item.CatalogItemID, item.Quantity, item.AcquiredDate);
        }
    }
}