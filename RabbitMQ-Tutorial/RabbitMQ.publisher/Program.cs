using RabbitMQ.Client;
using Shared;
using System.Text;
using System.Text.Json;

public class Program
{
    public enum LogNames
    {
        Critical=1,
        Error=2,
        Warning=3,
        Info=4

    }
    private static void Main(string[] args)
    {
        var factory = new ConnectionFactory();
        factory.Uri = new Uri("amqps://jamopzbo:wr6fN8dWljpKl8RsQb0Qc7mfEbGADjuV@moose.rmq.cloudamqp.com/jamopzbo");

        using var connection = factory.CreateConnection();
        var channel = connection.CreateModel();

        channel.ExchangeDeclare("header-exchange",durable:true,type:ExchangeType.Headers);

        Dictionary<string,object> headers = new Dictionary<string,object>();

        headers.Add("format", "pdf");
        headers.Add("shape2", "A4");

        var properties = channel.CreateBasicProperties();
        properties.Headers = headers;
        properties.Persistent = true; //Mesajlar kalıcı hale gelir.

        var product = new Product
        {
            Id = 1,
            Name = "Book",
            Price = 250,
            Stock = 2
        };

        var productJsonString = JsonSerializer.Serialize(product);




        channel.BasicPublish("header-exchange", string.Empty, properties, Encoding.UTF8.GetBytes(productJsonString));
        Console.WriteLine("Message has been sended");

        Console.ReadLine();
    }
}