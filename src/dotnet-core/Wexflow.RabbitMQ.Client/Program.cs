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

            //var factory = new ConnectionFactory() { HostName = "localhost" };
            var factory = new ConnectionFactory() { Uri = new Uri(uri) };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "wexflow",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                string message = GetMessage();
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                                     routingKey: "wexflow",
                                     basicProperties: null,
                                     body: body);

                Console.WriteLine(" [x] Sent {0}", message);
            }

            Console.WriteLine("Press any key to stop Wexflow.RabbitMQ.Client...");
            Console.ReadLine();
        }

        static string GetMessage()
        {
            //return "{" +
            //    "\"WorkflowId\": 138," +
            //    "\"Params\":" +
            //    "[" +
            //    "	{" +
            //    "		\"ParamName\": \"ListId\"," +
            //    "		\"ParamValue\": \"5a2b8615-af4d-4966-e051-08d7385c1609\"" +
            //    "	}," +
            //    "	{" +
            //    "		\"ParamName\": \"Payload\"," +
            //    "		\"ParamValue\": { \"59792\": \"destination\" }" +
            //    "	}," +
            //    "	{" +
            //    "		\"ParamName\": \"Mapping\"," +
            //    "		\"ParamValue\": { \"59793\": \"59792\" }" +
            //    "	}" +
            //    "]" +
            //    "}";

            return "{\"tenantId\": 79,\"payload\": {\"_id\":\"5d8e2b5368b2491918f57294\",\"59792\":\"destination\"},\"workflowId\": 138}";
        }
    }
}
