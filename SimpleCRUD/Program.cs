using MongoDB.Driver;
using SimpleCRUD.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers(); 

var connectionString = builder.Configuration.GetConnectionString("MongoDb");

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Строка подключения к MongoDB не найдена в конфигурации.");
}

var mongoClient = new MongoClient(connectionString);

var databaseName = builder.Configuration["MongoDbSettings:DatabaseName"] ?? "ShopApiDb";
var mongoDatabase = mongoClient.GetDatabase(databaseName);

builder.Services.AddSingleton(mongoDatabase.GetCollection<User>("Users"));
builder.Services.AddSingleton(mongoDatabase.GetCollection<Post>("Posts"));


var app = builder.Build();

app.MapControllers(); 

app.Run();