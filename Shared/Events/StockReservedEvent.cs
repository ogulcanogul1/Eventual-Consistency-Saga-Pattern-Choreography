using Shared.Messages;

namespace Shared.Events
{
    public class StockReservedEvent
    {
        public int BuyerId { get; set; }
        public int OrderId { get; set; }
        public decimal TotalPrice { get; set; }
        public List<OrderItemMessage> OrderItems { get; set; }
    }
}
