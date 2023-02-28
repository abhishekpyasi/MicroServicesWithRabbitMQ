using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Play.Common;

namespace Play.Catlog.Service.Entities
{

    public class Item : IEntity
    {
        public Item(Guid id, string name, string description, decimal price, DateTimeOffset createdDate)
        {
            Id = id;
            Name = name;
            Description = description;
            this.price = price;
            CreatedDate = createdDate;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public decimal price { get; set; }

        public DateTimeOffset CreatedDate { get; set; }



    }
}