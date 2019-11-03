using System;
using System.Configuration;
using System.IO;
using Wexflow.Core.Service.Client;
using Microsoft.Extensions.Configuration;
using System.Linq;
using Wexflow.Core.Service.Contracts;

namespace WexFlow.Client
{
    public class WexFlowManager
    {
        WexFlowConfig _configs;

        public WexFlowManager(WexFlowConfig configs)
        {
            _configs = configs;
            
        }

        /// <summary>
        /// Create Workflow in WexFlow
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        public int Create(string payload)
        {
            var client = new WexflowServiceClient(_configs.WexflowWebServiceUri);

            client.CreateWorkflow(_configs.Username, _configs.Password, payload);
            // return workflowId;
            return 0;
        }

        /// <summary>
        /// Get a workflow by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public WorkflowInfo Get(int id)
        {
            var client = new WexflowServiceClient(_configs.WexflowWebServiceUri);

            var workflows = client.Search(string.Empty, _configs.Username, _configs.Password);
            return workflows.FirstOrDefault(p => p.Id == id);
        }

        public void update(int id, string payload)
        {
            // Update Code
        }

        public void Delete(int id)
        {
            // Delete a workflow
        }
    }
}
