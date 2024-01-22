using Shared.Messages;

namespace Shared.Events;

public class StockReservedEvent
{
    public Guid OrderId { get; set; }
    public int UserId { get; set; }
    public double TotalPrice { get; set; }
    public List<OrderItemMessage> OrderItemMessages { get; set; }
    
}
