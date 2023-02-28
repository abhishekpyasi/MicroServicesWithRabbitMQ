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
        public static InventoryItemDto AsDto(this InventoryItem item, string name, string description)

        {

            return new InventoryItemDto(item.CatalogItemID, name, description, item.Quantity, item.AcquiredDate);
        }
    }
}