using MassTransit;
using Order.API.Contexts;
using Shared.Events;
using Shared.Messages;
using System.Net.Http.Headers;

namespace Order.API.Consumers
{
    public class PaymentCompletedEventConsumer(OrderAPIContext _context) : IConsumer<PaymentCompletedEvent>
    {
        public async Task Consume(ConsumeContext<PaymentCompletedEvent> context)
        {
           Models.Order order = await _context.Orders.FindAsync(context.Message.OrderId);

            order.OrderStatus = OrderStatus.Completed;

            await _context.SaveChangesAsync();


            Console.WriteLine($"Sipariş Id: {context.Message.OrderId}");
            foreach (OrderItemMessage orderItem in context.Message.OrderItems)
            {
                Console.WriteLine($"Ürün Id : {orderItem.ProductId}");
                Console.WriteLine($"Ürün Adet : {orderItem.Count}");
                Console.WriteLine($"Ürün Fiyat : {orderItem.Price}");
                Console.WriteLine($"Ürünün Toplam Fiyatı : {orderItem.Price * orderItem.Count}");
                Console.WriteLine("-----------------------------------------------------------------------");
            }

            Console.WriteLine($"Toplam Fiyat : {context.Message.TotalPrice}");

        }
    }
}
