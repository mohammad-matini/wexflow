using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System;
using System.Text;

namespace Wexflow.RabbitMQ.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            IConfiguration config = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
               .Build();
            var uri = config["CloudAmpqUrl"];

            var factory = new ConnectionFactory() { Uri = new Uri(uri) };
            //var factory = new ConnectionFactory() { HostName = "localhost" };
            var queueName = "Standard";
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: queueName,
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                string message = GetMessage();
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                                     routingKey: queueName,
                                     basicProperties: null,
                                     body: body);

                Console.WriteLine(" [x] Sent {0}", message);
            }

            Console.WriteLine("Press any key to stop Wexflow.RabbitMQ.Client...");
            Console.ReadLine();
        }

        static string GetMessage()
        {
            // Create
            return "{\"tenantId\": 79,\"payload\": {\"_id\":\"5d8e2b5368b2491918f57294\",\"59792\":\"destination\",\"59793\":\"123\"},\"workflowId\": 138}";
            // Update
            //return "{\"tenantId\": 79, \"recordId\": \"5d91be503aeff00001d26f84\", \"payload\": {\"_id\":\"5d8e2b5368b2491918f57294\",\"59792\":\"destination-updated\",\"59793\":\"desc-updated\"},\"workflowId\": 139}";
        }
    }
}
