using Microsoft.EntityFrameworkCore;
using Stock.API.Models;

namespace Stock.API.Contexts;

public class StockAPIContext: DbContext
{
    public StockAPIContext(DbContextOptions<StockAPIContext> context) : base(context)
    {
        
    }
    public DbSet<Models.Stock> Stocks { get; set; }
}
