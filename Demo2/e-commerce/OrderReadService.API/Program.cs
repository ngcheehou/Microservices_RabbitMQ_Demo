
using Ecommerce.Messaging;
using Microsoft.EntityFrameworkCore;
using OrderReadService.API.Infrastructure;
using OrderReadService.API.Messaging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddDbContext<OrderReadDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("OrderReadDb")));

builder.Services.AddRabbitMqOptions(builder.Configuration);
builder.Services.AddHostedService<OrderEventConsumer>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<OrderReadDbContext>();
    db.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
