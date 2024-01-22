using Shared.Messages;

namespace Shared.Events;

public class OrderCreatedEvent
{
    public Guid OrderId { get; set; }
    public int UserId { get; set; }
    public List<OrderItemMessage> OrderItemMessages { get; set; }
}