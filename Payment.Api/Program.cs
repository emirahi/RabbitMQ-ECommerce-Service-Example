using MassTransit;
using Payment.Api.Consumer;
using Shared.Config;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://localhost:5002/");

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMassTransit(configure =>
{
    configure.AddConsumer<StockReservedEventConsumer>();
    configure.UsingRabbitMq((context, config) =>
    {
        config.Host(new Uri(builder.Configuration["RabbitMQHost"]));
        
        config.ReceiveEndpoint(RabbitMQConfigure.Payment_StockReservedEventQueue,e => e.ConfigureConsumer<StockReservedEventConsumer>(context));
    });
});

var app = builder.Build();
app.Run();