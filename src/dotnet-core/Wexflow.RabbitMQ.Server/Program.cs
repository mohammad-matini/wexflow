using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using Wexflow.Core.Service.Client;

namespace Wexflow.RabbitMQ.Server
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
            var client = new WexflowServiceClient(config["WexflowWebServiceUri"]);
            var username = config["Username"];
            var password = config["Password"];
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "wexflow",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    try
                    {
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);
                        Console.WriteLine(" [x] Received {0}", message);

                        var o = JObject.Parse(message);
                        var workflowId = o.Value<int>("workflowId");
                        var payload = o.Value<JObject>("payload");

                        var parameters = "[{\"ParamName\":\"Payload\",\"ParamValue\":" + payload.ToString() + "}]";

                        client.StartWorkflow(workflowId, username, password, parameters);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                };
                channel.BasicConsume(queue: "wexflow",
                                     autoAck: true,
                                     consumer: consumer);


                Console.WriteLine("Press any key to stop Wexflow.RabbitMQ.Server...");
                Console.ReadKey();
            }
        }
    }
}
