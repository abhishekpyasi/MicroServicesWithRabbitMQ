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
    public class CatalogItemUpdatedConsumers : IConsumer<CatalogItemUpdated>
    {
        private IRepository<CatalogItem> repository;


        public CatalogItemUpdatedConsumers(IRepository<CatalogItem> repository)
        {
            this.repository = repository;
        }

        public async Task Consume(ConsumeContext<CatalogItemUpdated> context)
        {

            var message = context.Message;

            var item = await repository.GetItemAsync(message.ItemId);

            if (item == null)
            {
                item = new CatalogItem
                {

                    Id = message.ItemId,
                    Name = message.Name,
                    Description = message.Description
                };


                await repository.CreateAsync(item);
            }
            else
            {

                item.Name = message.Name;
                item.Description = message.Description;
                await repository.UpdateAsync(item);

            }






        }

    }
}