using MassTransit;
using Microsoft.EntityFrameworkCore;
using Order.API.Consumers;
using Order.API.Contexts;
using Order.API.Models;
using Order.API.ViewModels;
using Shared.Events;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMassTransit(configuration =>
{
    configuration.AddConsumer<StockNotReservedEventConsumer>();
    configuration.AddConsumer<PaymentCompletedEventConsumer>();
    configuration.AddConsumer<PaymentFailedEventConsumer>();
    configuration.UsingRabbitMq((context, _configure) =>
    {
        _configure.Host(builder.Configuration["RabbitMQ"]);

        _configure.ReceiveEndpoint(Shared.RabbitMQSettings.Order_StockNotReservedEventQueue,e => e.ConfigureConsumer<StockNotReservedEventConsumer>(context));

        _configure.ReceiveEndpoint(Shared.RabbitMQSettings.Order_PaymentCompletedEventQueue, e => e.ConfigureConsumer<PaymentCompletedEventConsumer>(context));

        _configure.ReceiveEndpoint(Shared.RabbitMQSettings.Order_PaymentFailedEventQueue, e => e.ConfigureConsumer<PaymentFailedEventConsumer>(context));
    });
});

builder.Services.AddDbContext<OrderAPIContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SQLServer"));
});

var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();



app.MapPost("/create-order", async (CreateOrderViewModel orderVM, OrderAPIContext context,IPublishEndpoint publishEndpoint) =>
{
    Order.API.Models.Order order = new()
    {
        BuyerId = orderVM.BuyerId,
        CreatedDate = DateTime.Now,
        OrderStatus = Order.API.OrderStatus.Pending,
        OrderItems = orderVM.OrderItems.Select(x => new OrderItem
        {
            Count = x.Count,
            Price = x.Price,
            ProductId = x.ProductId
        }).ToList(),
        TotalPrice = orderVM.OrderItems.Sum(x => x.Price * x.Count)
    };

    await context.AddAsync(order);
    await context.SaveChangesAsync();

    OrderCreatedEvent orderCreatedEvent = new()
    {
        BuyerId = order.BuyerId,
        OrderId = order.Id,
        OrderItems = order.OrderItems.Select(x => new Shared.Messages.OrderItemMessage
        {
            Count = x.Count,
            Price = x.Price,
            ProductId = x.ProductId
        }).ToList(),
         TotalPrice = order.TotalPrice
    };

    await publishEndpoint.Publish(orderCreatedEvent);
});

app.Run();
