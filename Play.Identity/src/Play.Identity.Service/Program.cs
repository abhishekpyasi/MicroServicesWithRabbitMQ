using System.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Play.Common.Settings;
using Play.Identity.Service;
using Play.Identity.Service.Entities;
using Play.Identity.Service.HostedService;
using Play.Identity.Service.Settings;


var builder = WebApplication.CreateBuilder(args);



// Add services to the container.

BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));

var serviceSettings = builder.Configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();
var mongoDbSettings = builder.Configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();
IdentityServerSettings? identityServerSettings = builder.Configuration.GetSection(nameof(IdentityServerSettings)).Get<IdentityServerSettings>();
//builder.Services.Configure<IdentitySettings>(builder.Configuration.GetSection(nameof(IdentitySettings)));
builder.Configuration.AddJsonFile("secrets.json", optional: true, reloadOnChange: true);
var secrets = new ConfigurationBuilder()
    .AddJsonFile("secrets.json", true, true)
    .Build();

string secretValue = secrets["IdentitySettings:AdminUserPassword"];


builder.Services.Configure<IdentitySettings>(builder.Configuration.GetSection(nameof(IdentitySettings)));
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
                .AddInMemoryClients(identityServerSettings?.Clients) // clients which can request
                .AddInMemoryIdentityResources(identityServerSettings?.IdentityResources) // named collection of claims
                .AddInMemoryApiResources(identityServerSettings?.ApiResources) // for aud claim generation for which resource you want token
                .AddDeveloperSigningCredential();







builder.Services.AddLocalApiAuthentication();// for authentication of API residing in IdentytyServer here its UserAPi

builder.Services.AddControllers();
builder.Services.AddHostedService<IdentitySeedHostedService>(); // to add hosted service to DI container


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
