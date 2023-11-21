using System.Configuration;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Play.Common.Settings;
using Play.Identity.Service.Entities;
using Play.Identity.Service.Settings;


var builder = WebApplication.CreateBuilder(args);



// Add services to the container.

BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));

var serviceSettings = builder.Configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();
var mongoDbSettings = builder.Configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();
IdentityServerSettings? identityServerSettings = builder.Configuration.GetSection(nameof(IdentityServerSettings)).Get<IdentityServerSettings>();


builder.Services.AddDefaultIdentity<ApplicationUser>()
.AddRoles<ApplicationRole>().AddMongoDbStores<ApplicationUser, ApplicationRole, Guid>(


mongoDbSettings?.ConnectionString, serviceSettings?.ServiceName
);
builder.Services.AddIdentityServer(options =>
{
    options.Events.RaiseErrorEvents = true;
    options.Events.RaiseSuccessEvents = true;
    options.Events.RaiseFailureEvents = true;

})
                .AddAspNetIdentity<ApplicationUser>()
                .AddInMemoryApiScopes(identityServerSettings?.ApiScopes)
                .AddInMemoryClients(identityServerSettings?.Clients)
                .AddInMemoryIdentityResources(identityServerSettings?.IdentityResources)
                .AddDeveloperSigningCredential();





builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseIdentityServer();

app.UseAuthorization();
#pragma warning disable ASP0014

app.UseEndpoints(endpoints =>
{

    endpoints.MapControllers();
    endpoints.MapRazorPages();


});

#pragma warning restore ASP0014

app.Run();
