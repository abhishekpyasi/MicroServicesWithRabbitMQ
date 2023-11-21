using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Play.Identity.Service.Entities;

namespace Play.Identity.Service.Controllers
{
    [ApiController]
    [Route("users")]
    public class UserController : ControllerBase
    {

        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(UserManager<ApplicationUser> userManager)
        {

            this._userManager = userManager;

        }
        [HttpGet]

        public ActionResult<IEnumerable<UserDto>> Get()
        {

            var users = _userManager?.Users.ToList().Select(user => user.AsDto());
            return Ok(users);

        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetByIdAsync(Guid id)
        {

            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {

                return NotFound();
            }

            return Ok(user.AsDto());
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync(Guid id, UpdateUserDto updateUserDto)
        {

            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
            {

                return NotFound();
            }

            user.Email = updateUserDto.Email;
            user.UserName = updateUserDto.Email;
            user.Gil = updateUserDto.Gil;

            await _userManager.UpdateAsync(user);
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {

            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
            {

                return NotFound();
            }

            await _userManager.DeleteAsync(user);

            return NoContent();


        }

    }
}