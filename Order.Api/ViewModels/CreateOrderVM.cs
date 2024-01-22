namespace Order.Api.ViewModels;

public class CreateOrderVM
{
    public int UserId { get; set; }
    public List<CreateOrderItemVm> Orders { get; set; }
}