using Econolite.Ode.Api.SpatCorridorSync;
using Econolite.Ode.Api.SpatCorridorSync.Services;
using Econolite.Ode.Authorization.Extensions;
using Econolite.Ode.Messaging.SpatCorridorSync.Extensions;
using Econolite.Ode.Persistence.Mongo;
using Repository.SpatCorridorSync.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMongo();

builder.Services.AddTokenHandler(options =>
{
    options.Authority = builder.Configuration.GetValue("Authentication:Authority", "https://keycloak.cosysdev.com/auth/realms/moundroad")!;
    options.ClientId = builder.Configuration.GetValue("Authentication:ClientId", "")!;
    options.ClientSecret = builder.Configuration.GetValue("Authentication:ClientSecret", "")!;
});
builder.Services.AddHttpClient<SyncService>("syncService", client => client.BaseAddress = new Uri(builder.Configuration["Sync:Url"] ?? throw new NullReferenceException("Configuration Sync:Url setting")));
builder.Services.AddSpatCorridorRepo();
builder.Services.AddSpatIntersectionRepo();
builder.Services.AddCorridorConsumer(_ => _.ConfigTopic = "Topics:SpatCorridorSync");
builder.Services.AddIntersectionConsumer(_ => _.ConfigTopic = "Topics:SpatIntersectionSync");

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHostedService<CorridorConsumerWorker>();
builder.Services.AddHostedService<IntersectionConsumerWorker>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
   app.UseHttpsRedirection(); 
}

app.UseAuthorization();

app.MapControllers();

app.Run();