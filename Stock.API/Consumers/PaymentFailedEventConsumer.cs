using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Events;
using Shared.Messages;
using Stock.API.Contexts;

namespace Stock.API.Consumers
{
    public class PaymentFailedEventConsumer(StockAPIContext _context,ISendEndpointProvider sendEndpointProvider) : IConsumer<PaymentFailedEvent>
    {
        public async Task Consume(ConsumeContext<PaymentFailedEvent> context)
        {
            // ödeme işlemi başarısız olduğunda düşürülen stock sayısı geri eklenmelidir.
            foreach (OrderItemMessage orderItem in context.Message.OrderItems)
            {
                Models.Stock stock = await _context.Stocks.Where(x => x.ProductId == orderItem.ProductId).FirstOrDefaultAsync();
                if (stock != null)
                {
                    stock.Count += orderItem.Count;
                }
            }

            await _context.SaveChangesAsync();

            PaymentFailedEvent paymentFailedEvent = new()
            {
                Description = "Ödeme Hatası",
                OrderId = context.Message.OrderId
            };

            ISendEndpoint sendEndpoint =  await sendEndpointProvider.GetSendEndpoint(new($"queue:{Shared.RabbitMQSettings.Order_PaymentFailedEventQueue}"));
            await sendEndpoint.Send(paymentFailedEvent); // Order Status'u Abort'a getirmek için
        }
    }
}
