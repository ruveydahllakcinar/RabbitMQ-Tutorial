using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared;
using System;
using System.Reflection;
using System.Text;
using System.Text.Json;

public class Program
{
    private static void Main(string[] args)
    {
        var factory = new ConnectionFactory();
        factory.Uri = new Uri("amqps://jamopzbo:wr6fN8dWljpKl8RsQb0Qc7mfEbGADjuV@moose.rmq.cloudamqp.com/jamopzbo");

        using var connection = factory.CreateConnection();
        var channel = connection.CreateModel();
        channel.ExchangeDeclare("header-exchange", durable: true, type: ExchangeType.Headers);

        channel.BasicQos(0, 1, false);
        var consumer = new EventingBasicConsumer(channel);
        var queueName = channel.QueueDeclare().QueueName;
        Dictionary<string, object> headers = new Dictionary<string, object>();

        headers.Add("format", "pdf");
        headers.Add("shape", "A4");
        headers.Add("x-match", "any");
        channel.QueueBind(queueName, "header-exchange",String.Empty,headers);
        channel.BasicConsume(queueName, false, consumer);
        Console.WriteLine("Listening logs...");

        consumer.Received += (object? sender, BasicDeliverEventArgs e) =>
        {
            var message = Encoding.UTF8.GetString(e.Body.ToArray());
            Product product = JsonSerializer.Deserialize<Product>(message);
            Thread.Sleep(1500);
            Console.WriteLine($"Incoming message: { product?.Id} - {product?.Name}- {product?.Price}- {product?.Stock}");

            
            channel.BasicAck(e.DeliveryTag, false);
        };
        Console.ReadLine();
    }

}

