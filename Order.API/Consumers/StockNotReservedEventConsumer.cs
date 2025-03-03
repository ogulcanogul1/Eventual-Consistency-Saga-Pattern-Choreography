using MassTransit;
using Microsoft.EntityFrameworkCore;
using Order.API.Contexts;
using Shared.Events;

namespace Order.API.Consumers
{
    public class StockNotReservedEventConsumer(OrderAPIContext _context) : IConsumer<StockNotReservedEvent>
    {
        public async Task Consume(ConsumeContext<StockNotReservedEvent> context)
        {
            Models.Order order =  await _context.Orders.Where(x => x.Id == context.Message.OrderId).FirstOrDefaultAsync();

            order!.OrderStatus = OrderStatus.Abort;

            await _context.SaveChangesAsync();
        }
    }
}
