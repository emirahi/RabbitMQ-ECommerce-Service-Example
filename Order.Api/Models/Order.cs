using Order.Api.Enum;

namespace Order.Api.Models;

public class Order
{
    public Guid Id { get; set; }
    public int UserId { get; set; }
    public OrderStatus Status { get; set; }
    public List<OrderItem> OrderItems { get; set; }
    public DateTime CreatedDateTime { get; set; }
}