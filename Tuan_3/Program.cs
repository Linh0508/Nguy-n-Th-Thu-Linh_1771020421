using HaiChanBank.Events;
using HaiChanBank.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// choose bus implementation by configuration
var useRabbit = builder.Configuration.GetValue<bool>("UseRabbitMq");
if (useRabbit)
{
    // read individual values (optional: support more connection options)
    var host = builder.Configuration["Rabbit:Host"] ?? "localhost";
    var port = builder.Configuration.GetValue<int?>("Rabbit:Port") ?? 5672;
    var user = builder.Configuration["Rabbit:User"] ?? "guest";
    var pass = builder.Configuration["Rabbit:Pass"] ?? "guest";

    builder.Services.AddSingleton<IEventBus>(_ => new RabbitMqEventBus(host, port, user, pass));
}
else
{
    builder.Services.AddSingleton<IEventBus, EventBus>();
}

// register services
builder.Services.AddSingleton<AccountService>();
builder.Services.AddSingleton<AnalyticService>();
builder.Services.AddSingleton<FraudDetectionService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
// If you use Swashbuckle, make sure the package is installed and uncomment the next line:
// builder.Services.AddSwaggerGen();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    // If Swashbuckle is installed, uncomment the next lines:
    // app.UseSwagger();
    // app.UseSwaggerUI();
}

app.MapControllers();
app.Run();