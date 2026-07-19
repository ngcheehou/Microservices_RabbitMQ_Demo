using Ecommerce.Messaging;
using Microsoft.EntityFrameworkCore;
using PaymentService.API.Application;
using PaymentService.API.ExternalServices;
using PaymentService.API.ExternalServices.Interfaces;
using PaymentService.API.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();


builder.Services.AddDbContext<PaymentDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("PaymentDb")));

builder.Services.AddHttpClient("OrderService", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:OrderService"]!);
});

builder.Services.AddSingleton<IPaymentGateway, MockPaymentGateway>();
builder.Services.AddScoped<IOrderClient, OrderClient>();
builder.Services.AddScoped<IPaymentAppService, PaymentAppService>();
builder.Services.AddRabbitMqPublisher(builder.Configuration);

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PaymentDbContext>();
    db.Database.EnsureCreated();
    //await DbInitializer.Seed(db, builder.Configuration);
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
 