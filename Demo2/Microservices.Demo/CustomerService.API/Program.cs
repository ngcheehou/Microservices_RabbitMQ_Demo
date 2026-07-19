using CustomerService.API.Application;
using CustomerService.API.Data;
using CustomerService.API.Infrastructure;
using Ecommerce.Messaging;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<CustomerDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CustomerDb")));

builder.Services.AddScoped<ICustomerAppService, CustomerAppService>();
builder.Services.AddRabbitMqPublisher(builder.Configuration);

var app = builder.Build();

// Ensure the database and table exist, then seed default data if empty.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CustomerDbContext>();
    db.Database.EnsureCreated();
    DbInitializer.Seed(db);
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
