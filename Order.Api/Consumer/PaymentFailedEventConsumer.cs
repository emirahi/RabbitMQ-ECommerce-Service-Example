using MassTransit;
using Order.Api.DataAccess;
using Order.Api.Enum;
using Shared.Events;

namespace Order.Api.Consumer;

public class PaymentFailedEventConsumer(OrderDbContext _context):IConsumer<PaymentFailedEvent>
{
    public async Task Consume(ConsumeContext<PaymentFailedEvent> context)
    {
        var order = await _context.Orders.FindAsync(context.Message.OrderId);
        if (order == null)
            throw new NullReferenceException();

        order.Status = OrderStatus.Failed;
        await _context.SaveChangesAsync();
    }
}