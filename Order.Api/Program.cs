using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Order.Api.Consumer;
using Order.Api.DataAccess;
using Order.Api.Enum;
using Order.Api.Models;
using Order.Api.ViewModels;
using Shared.Config;
using Shared.Events;
using Shared.Messages;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<OrderDbContext>(options => options.UseMySQL(builder.Configuration.GetConnectionString("Mysql")));
builder.Services.AddMassTransit(_configure =>
{
    _configure.AddConsumer<PaymentCompletedEventConsumer>();
    _configure.AddConsumer<PaymentFailedEventConsumer>();
    _configure.AddConsumer<StockNotReservedEventConsumer>();
    
    _configure.UsingRabbitMq((_context, _config) =>
    {
        _config.Host(new Uri(builder.Configuration["RabbitMQHost"]));
        
        _config.ReceiveEndpoint(RabbitMQConfigure.Order_PaymentFailedEventQueue,e => e.ConfigureConsumer<PaymentFailedEventConsumer>(_context));
        _config.ReceiveEndpoint(RabbitMQConfigure.Order_PaymentCompletedEventQueue,e => e.ConfigureConsumer<PaymentCompletedEventConsumer>(_context));
        _config.ReceiveEndpoint(RabbitMQConfigure.Order_StockNotReservedEventQueue,e => e.ConfigureConsumer<StockNotReservedEventConsumer>(_context));
        
    });
    
});

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

app.MapPost("/Create-Order", async (CreateOrderVM orderVm,OrderDbContext _dbContext,IPublishEndpoint _publishEndpoint) =>
{
    Order.Api.Models.Order order = new()
    {
        UserId = orderVm.UserId,
        Status = OrderStatus.Created,
        CreatedDateTime = DateTime.UtcNow, 
        // Automapper ile bir dönüşüm gerçekleştirilebilirdi ancak sadece rabbitmq üzerinde bir işlem gerçekleştirmek istiyorum
        OrderItems = orderVm.Orders.Select(oi => new OrderItem() 
        {
            ProductId = oi.ProductId,
            Count = oi.Count
        }).ToList()
    };

    await _dbContext.Orders.AddAsync(order);
    await _dbContext.SaveChangesAsync();

    await _publishEndpoint.Publish<OrderCreatedEvent>(new OrderCreatedEvent()
    {
        UserId = order.UserId,
        OrderId = order.Id,
        OrderItemMessages = order.OrderItems.Select(oi => new OrderItemMessage()
        {
            ProductId = oi.ProductId,
            Count = oi.Count
        }).ToList()
    });

});


app.Run();




