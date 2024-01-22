using MassTransit;
using Order.Api.DataAccess;
using Order.Api.Enum;
using Shared.Events;

namespace Order.Api.Consumer;

public class PaymentCompletedEventConsumer(OrderDbContext _context):IConsumer<PaymentCompletedEvent>
{
    public async Task Consume(ConsumeContext<PaymentCompletedEvent> context)
    {
        var order = await _context.Orders.FindAsync(context.Message.OrderId);
        if (order == null)
            throw new NullReferenceException();

        order.Status = OrderStatus.Complated;
        await _context.SaveChangesAsync();
    }
}