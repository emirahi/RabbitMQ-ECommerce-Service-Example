using MassTransit;
using MongoDB.Driver;
using Shared.Config;
using Shared.Events;
using Shared.Messages;
using Stock.Api.DataAccess;

namespace Stock.Api.Consumer;

public class OrderCreatedEventConsumer(MongoDBService mongoDbService,ISendEndpointProvider sendEndpointProvider, IPublishEndpoint publishEndpoint):IConsumer<OrderCreatedEvent>
{
    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        List<bool> stockResult = new();
        IMongoCollection<Models.Stock> collection = mongoDbService.GetCollection<Models.Stock>();

        foreach (OrderItemMessage orderItemMessage in context.Message.OrderItemMessages)
        {
            stockResult.Add(await (await collection.FindAsync(s => s.ProductId == orderItemMessage.ProductId && s.Count >= orderItemMessage.Count)).AnyAsync());
        }

        double totalPrice = 0.0; 
        if (stockResult.TrueForAll(s => s.Equals(true)))
        {
            foreach (OrderItemMessage orderItemMessage in context.Message.OrderItemMessages)
            {
                Models.Stock stock = await (await collection.FindAsync(x => x.ProductId == orderItemMessage.ProductId))
                    .FirstOrDefaultAsync();
                stock.Count -= orderItemMessage.Count;
                totalPrice += stock.Price;
                await collection.FindOneAndReplaceAsync(x => x.ProductId == orderItemMessage.ProductId, stock);
            }

            var sendEndPoint = await sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{RabbitMQConfigure.Payment_StockReservedEventQueue}"));
            await sendEndPoint.Send(new StockReservedEvent()
            {
                OrderId = context.Message.OrderId,
                UserId = context.Message.UserId,
                TotalPrice = totalPrice,
                OrderItemMessages = context.Message.OrderItemMessages
            });
        }
        else
        {
            StockNotReservedEvent stockNotReservedEvent = new()
            {
                OrderId = context.Message.OrderId,
                UserId = context.Message.UserId,
                Message = "Stok Yeterli Değil"
            };

            await publishEndpoint.Publish(stockNotReservedEvent);
        }
    }
}



