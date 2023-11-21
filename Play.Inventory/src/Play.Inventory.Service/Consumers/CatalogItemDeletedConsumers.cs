using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Play.Common;
using Play.Inventory.Service.Entities;
using static Play.Catalog.Contract.Contracts;

namespace Play.Inventory.Service.Consumers
{
    public class CatalogItemDeletedConsumers : IConsumer<CatalogItemUpdated>
    {
        private IRepository<CatalogItem> repository;


        public CatalogItemDeletedConsumers(IRepository<CatalogItem> repository)
        {
            this.repository = repository;
        }

        public async Task Consume(ConsumeContext<CatalogItemUpdated> context)
        {

            var message = context.Message;

            var item = await repository.GetItemAsync(message.ItemId);

            if (item == null)
            {

                return;
            }

            await repository.RemoveAsync(message.ItemId);






        }

    }
}