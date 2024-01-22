namespace Order.Api.ViewModels;

public class OrderVM
{
    public int UserId { get; set; }
    public List<Models.Order> Orders { get; set; }
}