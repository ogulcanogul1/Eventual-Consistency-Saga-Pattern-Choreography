using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Events;
using Shared.Messages;
using Stock.API.Contexts;

namespace Stock.API.Consumers
{
    public class OrderCreatedEventConsumer : IConsumer<OrderCreatedEvent>
    {
        StockAPIContext _context;
        ISendEndpointProvider _sendEndpointProvider;
        IPublishEndpoint _publishEndpoint;
        public OrderCreatedEventConsumer(StockAPIContext context,ISendEndpointProvider sendEndpointProvider,IPublishEndpoint publishEndpoint)
        {
            _context = context;
            _sendEndpointProvider = sendEndpointProvider;
            _publishEndpoint = publishEndpoint; 
        }
        public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
            List<bool> results = new();

            foreach (OrderItemMessage orderItem in context.Message.OrderItems)
            {
                bool result = _context.Stocks.Where(x => x.ProductId == orderItem.ProductId && x.Count >= orderItem.Count).Any();

                results.Add(result);
            }

            if (results.TrueForAll(x => x.Equals(true)))
            {
                // Stock kontrol başarılı
                // ilem sonrası PaymentAPI'yi uyarıcak event fırlatılacak
                foreach (OrderItemMessage orderItem in context.Message.OrderItems)
                {
                    Models.Stock stock = await _context.Stocks.Where(x => x.ProductId == orderItem.ProductId).FirstOrDefaultAsync();

                    stock!.Count -= orderItem.Count;

                    await _context.SaveChangesAsync();
                }

                StockReservedEvent stockReservedEvent = new()
                {
                    BuyerId = context.Message.BuyerId,
                    OrderId = context.Message.OrderId,
                    OrderItems = context.Message.OrderItems,
                    TotalPrice = context.Message.TotalPrice
                };
                 ISendEndpoint sendEndpoint =  await _sendEndpointProvider.GetSendEndpoint(new($"queue:{Shared.RabbitMQSettings.Payment_StockReservedEventQueue}"));

                await sendEndpoint.Send(stockReservedEvent);
            }
            else
            {
                // Stok kontrol işlemi başarısız
                // OrderAPI uyarılırak (event fırlatılarak) OrderStatu Abort'a çekilecek.

                StockNotReservedEvent stockNotReservedEvent = new()
                {
                    OrderId = context.Message.OrderId
                };

                 await _publishEndpoint.Publish(stockNotReservedEvent);
            }

        }
    }
}
