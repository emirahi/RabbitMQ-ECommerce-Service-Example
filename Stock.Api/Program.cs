using MassTransit;
using MongoDB.Driver;
using Shared.Config;
using Stock.Api.Consumer;
using Stock.Api.DataAccess;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://localhost:5001/");

builder.Services.AddSingleton<MongoDBService>();

builder.Services.AddMassTransit(configure =>
{
    configure.AddConsumer<OrderCreatedEventConsumer>();
    configure.AddConsumer<PaymentFailedEventConsumer>();
    
    configure.UsingRabbitMq((context, config) =>
    {
        config.Host(new Uri(builder.Configuration["RabbitMQHost"]));
        config.ReceiveEndpoint(RabbitMQConfigure.Stock_OrderCreatedEventQueue,e => e.ConfigureConsumer<OrderCreatedEventConsumer>(context));
        config.ReceiveEndpoint(RabbitMQConfigure.Stock_PaymentFailedEventQueue,e => e.ConfigureConsumer<PaymentFailedEventConsumer>(context));
    });
    
});

var app = builder.Build();
using IServiceScope scope = app.Services.CreateScope();
MongoDBService mongoDbService = scope.ServiceProvider.GetService<MongoDBService>();
var stockCollection = mongoDbService.GetCollection<Stock.Api.Models.Stock>();
if (!stockCollection.FindSync(x => true).Any())
{
    await stockCollection.InsertOneAsync(new Stock.Api.Models.Stock() { ProductId = 1, Count = 30, Price = 149.99 });
    await stockCollection.InsertOneAsync(new Stock.Api.Models.Stock() { ProductId = 2, Count = 300, Price = 149.99 });
    await stockCollection.InsertOneAsync(new Stock.Api.Models.Stock() { ProductId = 3, Count = 230, Price = 149.99 });
    await stockCollection.InsertOneAsync(new Stock.Api.Models.Stock() { ProductId = 4, Count = 330, Price = 149.99 });
    await stockCollection.InsertOneAsync(new Stock.Api.Models.Stock() { ProductId = 5, Count = 130, Price = 149.99 });
}


app.Run();

