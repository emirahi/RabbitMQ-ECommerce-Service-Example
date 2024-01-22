using MassTransit;
using Shared.Config;
using Shared.Events;

namespace Payment.Api.Consumer;

public class StockReservedEventConsumer(IPublishEndpoint publishEndpoint):IConsumer<StockReservedEvent>
{
    public async  Task Consume(ConsumeContext<StockReservedEvent> context)
    {
        if (context.Message.TotalPrice <= 0)
            throw new Exception();

        if (false)
        {
            
            publishEndpoint.Publish(new PaymentCompletedEvent()
            {
                OrderId = context.Message.OrderId,
                UserId = context.Message.UserId,
                Message = "Ödeme Başarılı"
            });
        }
        else
        {
            publishEndpoint.Publish(new PaymentFailedEvent()
            {
                OrderId = context.Message.OrderId,
                UserId = context.Message.UserId,
                OrderItemMessages = context.Message.OrderItemMessages
            });
        }
        
    }
}