using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Play.Identity.Service.Entities;

namespace Play.Identity.Service
{
    public static class Extension
    {
        public static UserDto AsDto(this ApplicationUser user)
        {

            return new UserDto(


                user.Id,
                user.Email,
                user.UserName,
                user.Gil,
                user.CreatedOn

                );




        }
    }
}