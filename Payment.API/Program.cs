using MassTransit;
using Payment.API.Consumers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMassTransit(configurator =>
{
    configurator.AddConsumer<StockReservedEventConsumer>();
    configurator.UsingRabbitMq((context, _configurator) =>
    {
        _configurator.Host(builder.Configuration["RabbitMQ"]);
        _configurator.ReceiveEndpoint(Shared.RabbitMQSettings.Payment_StockReservedEventQueue,e => e.ConfigureConsumer<StockReservedEventConsumer>(context));
    });
});

var app = builder.Build();





app.Run();
