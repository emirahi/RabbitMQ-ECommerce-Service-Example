namespace Shared.Events;

public class PaymentCompletedEvent
{
    public Guid OrderId { get; set; }
    public int UserId { get; set; }
    public string Message { get; set; }
}