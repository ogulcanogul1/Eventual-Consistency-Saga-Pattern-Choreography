﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared;

public static class RabbitMQSettings
{
    public const string Stock_OrderCratedEventQueue = "stock-order-created-event-queue";
    public const string Payment_StockReservedEventQueue = "payment-stock-reserved-event-queue";
    public const string Order_StockNotReservedEventQueue = "order-stock-not-reserved-event-queue";
    public const string Order_PaymentCompletedEventQueue = "order-payment-completed-event-queue";
    public const string Order_PaymentFailedEventQueue = "order-payment-failed-event-queue";
    public const string Stock_PaymentFailedEventQueue = "payment-payment-failed-event-queue";
}
