using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Play.Identity.Service.Entities;

/*This class is for creating a hosted service which will create Player and Admin user if they are not created
we need admin and player user for admin and player so that we can have claim based authorization based on role admin and player
*/
namespace Play.Identity.Service.HostedService
{
    public class IdentitySeedHostedService : IHostedService
    {

        private readonly IServiceScopeFactory? serviceScopeFactory; // to get access to user manager and role manager objects
        private readonly IdentitySettings? settings;

        public IdentitySeedHostedService(IServiceScopeFactory? serviceScopeFactory,
        IOptions<IdentitySettings>? identityOptions)
        {
            this.serviceScopeFactory = serviceScopeFactory;

            settings = identityOptions?.Value;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {

            using var scope = serviceScopeFactory?.CreateScope(); // to create scope for 
            var roleManager = scope?.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            var userManager = scope?.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            await CreateRoleIfNotExistsAsync(Roles.Admin, roleManager!);
            await CreateRoleIfNotExistsAsync(Roles.Player, roleManager!);

            var adminUser = await userManager.FindByEmailAsync(settings?.AdminUserEmail!);
            if (adminUser == null)

            {

                adminUser = new ApplicationUser()
                {

                    UserName = settings!.AdminUserEmail,
                    Email = settings!.AdminUserEmail,
                };




                await userManager.CreateAsync(adminUser, settings.AdminUserPassword);
                await userManager.AddToRoleAsync(adminUser, Roles.Admin);
            }


        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
        }

        private static async Task CreateRoleIfNotExistsAsync(string role, RoleManager<ApplicationRole> roleManager)
        {

            var roleExists = await roleManager.RoleExistsAsync(role);
            if (!roleExists)
            {

                await roleManager.CreateAsync(new ApplicationRole { Name = role });
            }

        }
    }
}