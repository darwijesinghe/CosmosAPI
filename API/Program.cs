using API.Services;
using Microsoft.Azure.Cosmos;

var builder = WebApplication.CreateBuilder(args);
var config  = builder.Configuration;

// Add services to the container.

// Add CosmosDb client
builder.Services.AddSingleton(sp => new CosmosClient(config["CosmosDb:AccountEndpoint"], config["CosmosDb:AuthKey"]));

// Add the database service
builder.Services.AddSingleton(sp =>
{
    var client = sp.GetRequiredService<CosmosClient>();
    return new CosmosService(client, config["CosmosDb:DatabaseName"]);
});

// Gets the allowed containers
var allowedContainers = builder.Configuration.GetSection("CosmosDb:AllowedContainers").Get<HashSet<string>>();
builder.Services.AddSingleton<IReadOnlySet<string>>(allowedContainers);

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

app.UseAuthorization();

app.MapControllers();

app.Run();
