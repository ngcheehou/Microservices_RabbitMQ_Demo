
using Ecommerce.Messaging;
using Microsoft.EntityFrameworkCore;
using PaymentReadService.API.Infrastructure;
using PaymentReadService.API.Messaging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddDbContext<PaymentReadDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("PaymentReadDb")));

builder.Services.AddRabbitMqOptions(builder.Configuration);
builder.Services.AddHostedService<PaymentEventConsumer>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PaymentReadDbContext>();
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
