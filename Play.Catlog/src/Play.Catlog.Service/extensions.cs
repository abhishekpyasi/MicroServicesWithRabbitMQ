using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Play.Catlog.Service.DTO;
using Play.Catlog.Service.Entities;

namespace Play.Catlog.Service
{
    public static class extensions
    {

        public static ItemDto AsDto(this Item item)
        {


            return new ItemDto(item.Id, item.Name, item.Description, item.price, item.CreatedDate);
        }


    }
}