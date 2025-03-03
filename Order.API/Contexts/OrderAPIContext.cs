using Microsoft.EntityFrameworkCore;
using Order.API.Models;

namespace Order.API.Contexts
{
    public class OrderAPIContext : DbContext
    {
        public OrderAPIContext(DbContextOptions<OrderAPIContext> context) : base(context)
        {
            
        }
        public DbSet<Models.Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
    }
}
