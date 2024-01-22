using MassTransit;
using MongoDB.Driver;
using Shared.Events;
using Shared.Messages;
using Stock.Api.DataAccess;

namespace Stock.Api.Consumer;

public class PaymentFailedEventConsumer(MongoDBService mongoDbService,IPublishEndpoint publishEndpoint):IConsumer<PaymentFailedEvent>
{
    public async Task Consume(ConsumeContext<PaymentFailedEvent> context)
    {
        IMongoCollection<Models.Stock> collection = mongoDbService.GetCollection<Models.Stock>();

        foreach (OrderItemMessage orderItemMessage in context.Message.OrderItemMessages)
        {
            Models.Stock stock = await (await collection.FindAsync(x => x.ProductId == orderItemMessage.ProductId)).FirstOrDefaultAsync();
            if (stock != null)
            {
                stock.Count += orderItemMessage.Count;
                await collection.FindOneAndReplaceAsync(x => x.ProductId == orderItemMessage.ProductId, stock);   
            }
        }

    }
}