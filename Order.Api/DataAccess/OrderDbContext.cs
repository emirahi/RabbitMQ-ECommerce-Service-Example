using Microsoft.EntityFrameworkCore;

namespace Order.Api.DataAccess;

public class OrderDbContext:DbContext
{
    public OrderDbContext(DbContextOptions options):base(options)
    {
        
    }
    public DbSet<Models.Order> Orders { get; set; }
    
}