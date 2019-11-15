using log4net;
using log4net.Config;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using Wexflow.Core;
using Wexflow.Core.Service.Client;

namespace Wexflow.Server
{
    public class WexflowServer
    {
        public static IConfiguration Config;
        public static WexflowEngine WexflowEngine;

        public static void Main(string[] args)
        {
            try
            {
                Config = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .Build();

                int port = int.Parse(Config["WexflowServicePort"]);

                XmlDocument log4NetConfig = new XmlDocument();
                log4NetConfig.Load(File.OpenRead("log4net.config"));
                var repo = LogManager.CreateRepository(Assembly.GetEntryAssembly(), typeof(log4net.Repository.Hierarchy.Hierarchy));
                XmlConfigurator.Configure(repo, log4NetConfig["log4net"]);

                string wexflowSettingsFile = Config["WexflowSettingsFile"];
                WexflowEngine = new WexflowEngine(wexflowSettingsFile
                    , Config["WorkiomAuthUrl"]
                    , Config["CreateRecordUrl"]
                    , Config["UpdateRecordUrl"]
                    , Config["NotifyUserUrl"]);

                WexflowEngine.Run();

                var uri = Config["CloudAmpqUrl"];
                var queueName = Config["QueueName"];
                var factory = new ConnectionFactory() { Uri = new Uri(uri) };
                //var factory = new ConnectionFactory() { HostName = "localhost" };
                //var wexflowWebServiceUri = string.Format("http://localhost:{0}/wexflow/", port);
                var wexflowWebServiceUri = Config["WexflowWebServiceUri"];
                var client = new WexflowServiceClient(wexflowWebServiceUri);
                var username = Config["Username"];
                var password = Config["Password"];
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: queueName,
                                         durable: true,
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
                            Logger.InfoFormat("Received: {0}", message);

                            var o = JObject.Parse(message);
                            var workflowId = o.Value<int>("workflowId");
                            var payload = o.Value<JObject>("payload");

                            var parameters =
                            "[" +
                                "{\"ParamName\":\"Payload\",\"ParamValue\":" + (payload == null ? "\"\"" : payload.ToString()) + "}" +
                            "]";

                            var started = client.StartWorkflow(workflowId, username, password, parameters);

                            if (started)
                            {
                                Logger.InfoFormat("Workflow {0} started.", workflowId);
                            }
                            else
                            {
                                Logger.ErrorFormat("Workflow {0} not started. Error: Unauthorized.", workflowId);
                            }
                        }
                        catch (Exception e)
                        {
                            Logger.Error("An error occured while consuming queue message.", e);
                        }
                    };
                    channel.BasicConsume(queue: queueName,
                                         autoAck: true,
                                         consumer: consumer);

                    var host = new WebHostBuilder()
                        .UseContentRoot(Directory.GetCurrentDirectory())
                        .UseKestrel((context, options) =>
                        {
                            options.ListenAnyIP(port);
                        })
                        .UseStartup<Startup>()
                        .Build();

                    host.Run();
                }

            }
            catch (Exception e)
            {
                Logger.Error("An error occured while starting Wexflow server.", e);
            }

            Console.WriteLine();
            Console.Write("Press any key to stop Wexflow server...");
            Console.ReadKey();

        }
    }
}
