using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;



Console.WriteLine("RabbitMQ Demo");


var factory = new ConnectionFactory() { HostName = "localhost" };

using var connection = await factory.CreateConnectionAsync();

using var channel = await connection.CreateChannelAsync();
using var channel2 = await connection.CreateChannelAsync();
using var channel3 = await connection.CreateChannelAsync();

await channel.ExchangeDeclareAsync("my_exchange", ExchangeType.Direct);
//await channel2.ExchangeDeclareAsync("my_exchange2", ExchangeType.Direct);
//await channel3.ExchangeDeclareAsync("my_exchange3", ExchangeType.Direct);

 
await channel.QueueDeclareAsync("my_queue", true, false, false, null);
//await channel2.QueueDeclareAsync("my_queue2", true, false, false, null);
//await channel3.QueueDeclareAsync("my_queue3", true, false, false, null); 

// Create a binding between the exchange and the queue
await channel.QueueBindAsync("my_queue", "my_exchange", "my_routing_key", null);
//await channel2.QueueBindAsync("my_queue2", "my_exchange2", "my_routing_key2", null);
//await channel3.QueueBindAsync("my_queue3", "my_exchange3", "my_routing_key3", null);


Console.WriteLine("Infrastructure setup complete.");


//publish message
var message = "Hello, RabbitMQ!";
var body = Encoding.UTF8.GetBytes(message);

var message2 = "Hello, RabbitMQ 2!";
var body2 = Encoding.UTF8.GetBytes(message2);

var message3 = "Hello, RabbitMQ 3!";
var body3 = Encoding.UTF8.GetBytes(message3);


// Use named arguments for clarity and to avoid overload errors
await channel.BasicPublishAsync(
    exchange: "my_exchange",
    routingKey: "my_routing_key",
    mandatory: false,
    basicProperties: new BasicProperties(),
    body: body
);

//await channel2.BasicPublishAsync(
//    exchange: "my_exchange2",
//    routingKey: "my_routing_key2",
//    mandatory: false,
//    basicProperties: new BasicProperties(),
//    body: body2
//);


//await channel3.BasicPublishAsync(
//    exchange: "my_exchange3",
//    routingKey: "my_routing_key3",
//    mandatory: false,
//    basicProperties: new BasicProperties(),
//    body: body3
//);

Console.WriteLine("Message Published.");

// Start consuming
Console.WriteLine("Proceed to read the message");


string queueName = "my_queue";
string queueName2 = "my_queue2";    
string queueName3 = "my_queue3";

var consumer = new AsyncEventingBasicConsumer(channel);
consumer.ReceivedAsync += HandleReceivedMessage;//ask HandleReceivedMessage method to handle it


//var consumer2 = new AsyncEventingBasicConsumer(channel2);
//consumer2.ReceivedAsync += HandleReceivedMessage;//ask HandleReceivedMessage method to handle it

//var consumer3 = new AsyncEventingBasicConsumer(channel3);
//consumer3.ReceivedAsync += HandleReceivedMessage;//ask HandleReceivedMessage method to handle it

//"RabbitMQ, please notify me whenever a message arrives in my_queue. When it happens, call this method."
await channel.BasicConsumeAsync(queueName, autoAck: true, consumer: consumer); 
//await channel2.BasicConsumeAsync(queueName2, autoAck: true, consumer: consumer2);
//await channel3.BasicConsumeAsync(queueName3, autoAck: true, consumer: consumer3);   



Console.ReadLine();

static async Task HandleReceivedMessage(object sender, BasicDeliverEventArgs ea)
{
 
    //if (routingKey == "1")  

    // You can perform awaitable tasks inside here if needed
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);

    Console.WriteLine($"Method HandleReceivedMessage activate! Received message: {message}");

    // Explicitly return a completed task
    await Task.CompletedTask;
}