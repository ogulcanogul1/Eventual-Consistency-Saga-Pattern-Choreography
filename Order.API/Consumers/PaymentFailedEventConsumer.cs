using MassTransit;
using Order.API.Contexts;
using Shared.Events;

namespace Order.API.Consumers
{
    public class PaymentFailedEventConsumer(OrderAPIContext _context) : IConsumer<PaymentFailedEvent>
    {
        public async Task Consume(ConsumeContext<PaymentFailedEvent> context)
        {
            Models.Order order = await _context.Orders.FindAsync(context.Message.OrderId);

            order.OrderStatus = OrderStatus.Abort;

            await _context.SaveChangesAsync();    

            Console.WriteLine(context.Message.Description);
        }
    }
}
