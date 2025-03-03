using MassTransit;
using Shared.Events;

namespace Payment.API.Consumers
{
    public class StockReservedEventConsumer(IPublishEndpoint publishEndpoint) : IConsumer<StockReservedEvent>
    {
        public Task Consume(ConsumeContext<StockReservedEvent> context)
        {
            // Payment işlemleri ...


            if (true)
            {
                // Ödeme İşlemi başarılı ise OrderAPI' ye event yollaranarak orderstatus Completed yapılacak

                PaymentCompletedEvent paymentCompletedEvent = new()
                {
                    BuyerId = context.Message.BuyerId,
                    OrderId = context.Message.OrderId,
                    OrderItems = context.Message.OrderItems,
                    TotalPrice = context.Message.TotalPrice
                };

                publishEndpoint.Publish(paymentCompletedEvent);

            }
            else
            {
                // Ödeme İşlemi başarılı değil ise OrderAPI' ye event yollaranarak orderstatus Abort yapılacak

                PaymentFailedEvent paymentFailedEvent = new()
                {
                    OrderId = context.Message.OrderId,
                    Description = "Ödeme işlemi bşaarısız",
                     OrderItems = context.Message.OrderItems
                };
                publishEndpoint.Publish(paymentFailedEvent);
            }

            return Task.CompletedTask;
        }
    }
}
