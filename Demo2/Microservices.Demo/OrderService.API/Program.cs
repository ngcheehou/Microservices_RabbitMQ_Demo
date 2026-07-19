
using Ecommerce.Messaging;
using Microsoft.EntityFrameworkCore;
using OrderService.API.Application;
using OrderService.API.Data;
using OrderService.API.ExternalServices;
using OrderService.API.ExternalServices.Interfaces;
using OrderService.API.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();


builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("OrderDb")));

builder.Services.AddHttpClient("CustomerService", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:CustomerService"]!);
});

builder.Services.AddHttpClient("InventoryService", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:InventoryService"]!);
});

builder.Services.AddScoped<IOrderAppService, OrderAppService>();
builder.Services.AddScoped<ICustomerClient, CustomerClient>();
builder.Services.AddScoped<IInventoryClient, InventoryClient>();
builder.Services.AddRabbitMqPublisher(builder.Configuration);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
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
