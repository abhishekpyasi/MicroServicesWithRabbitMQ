using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Play.Catalog.Contract
{
    public class Contracts
    {
        public record CatalogItemCreated(Guid ItemId, string Name, string Description);
        public record CatalogItemUpdated(Guid ItemId, string Name, string Description);
        public record CatalogItemDeleted(Guid ItemId);


    }
}