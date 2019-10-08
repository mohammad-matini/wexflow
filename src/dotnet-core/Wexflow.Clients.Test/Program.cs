using Microsoft.Extensions.Configuration;
using System;

namespace Wexflow.Core.Service.Client.Test
{
    class Program
    {
        private static readonly string createPayload = "{\"Username\":\"admin\",\"Password\":\"ee05eaaba7b76f16e285d983d605c9bf\",\"Id\":999,\"WorkflowInfo\":{\"Id\":999,\"Name\":\"Workflow_WorkiomCreateRecord\",\"LaunchType\":1,\"Period\":\"00.00:00:00\",\"CronExpression\":null,\"IsEnabled\":true,\"Description\":\"Workflow_WorkiomCreateRecord\",\"IsNew\":false,	  \"HasRestParams\":true,	  \"WorkiomAuthUrl\":\"http://api.workiom.club:88/api/TokenAuth/Authenticate\",	  \"WorkiomUsername\":\"admin\",	  \"WorkiomPassword\": \"alarm1\",	  \"WorkiomTenantName\": \"demo\",\"LocalVariables\":[]},\"Tasks\":[{\"Id\":1,\"Name\":\"WorkiomCreateRecord\",\"Description\":\"Creating Workiom record\",\"IsEnabled\":true,\"Settings\":[{\"Name\":\"createRecordUrl\",\"Value\":\"http://api.workiom.club:88/api/services/app/Data/Create?listId=\",\"Attributes\":[]},{\"Name\":\"listId\",\"Value\":\"15db9ad3-e435-4b2b-170e-08d7363a65e1\",\"Attributes\":[]},{\"Name\":\"mapping\",\"Value\":\"[]\",\"Attributes\":[]}]}]}";
        private static readonly string updatedPayload = "{\"Username\":\"admin\",\"Password\":\"ee05eaaba7b76f16e285d983d605c9bf\",\"Id\":999,\"WorkflowInfo\":{\"Id\":999,\"Name\":\"Workflow_WorkiomCreateRecord2\",\"LaunchType\":1,\"Period\":\"00.00:00:00\",\"CronExpression\":null,\"IsEnabled\":true,\"Description\":\"Workflow_WorkiomCreateRecord2\",\"IsNew\":false,	  \"HasRestParams\":true,	  \"WorkiomAuthUrl\":\"http://api.workiom.club:88/api/TokenAuth/Authenticate\",	  \"WorkiomUsername\":\"admin\",	  \"WorkiomPassword\": \"alarm1\",	  \"WorkiomTenantName\": \"demo\",\"LocalVariables\":[]},\"Tasks\":[{\"Id\":1,\"Name\":\"WorkiomCreateRecord\",\"Description\":\"Creating Workiom record\",\"IsEnabled\":true,\"Settings\":[{\"Name\":\"createRecordUrl\",\"Value\":\"http://api.workiom.club:88/api/services/app/Data/Create?listId=\",\"Attributes\":[]},{\"Name\":\"listId\",\"Value\":\"15db9ad3-e435-4b2b-170e-08d7363a65e1\",\"Attributes\":[]},{\"Name\":\"mapping\",\"Value\":\"[]\",\"Attributes\":[]}]}]}";

        static void Main(string[] args)
        {
            try
            {
                IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

                var wexflowServiceUri = config["WexflowWebServiceUri"];
                var username = config["Username"];
                var password = config["Password"];

                var client = new WexflowServiceClient(wexflowServiceUri);

                // Test create
                var res = client.CreateWorkflow(username, password, createPayload);
                Console.WriteLine("Create result: {0}", res);

                // Test update
                res = client.UpdateWorkflow(username, password, updatedPayload);
                Console.WriteLine("Update result: {0}", res);

                // Test read
                var xml = client.ReadWorkflow(username, password, 999);
                Console.WriteLine("Read result: {0}", xml);

                // Test delete
                res = client.DeleteWorkflow(username, password, 999);
                Console.WriteLine("Delete result: {0}", res);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e);
            }
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
