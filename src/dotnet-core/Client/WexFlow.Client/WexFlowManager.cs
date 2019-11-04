using Newtonsoft.Json.Linq;
using System.Linq;
using Wexflow.Core.Service.Client;
using Wexflow.Core.Service.Contracts;

namespace WexFlow.Client
{
    public class WexFlowManager
    {
        private WexFlowConfig _configs;

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

            var created = client.CreateWorkflow(_configs.Username, _configs.Password, payload);

            // return workflowId
            if (created)
            {
                var o = JObject.Parse(payload);
                var wi = o.Value<JObject>("WorkflowInfo");
                var id = wi.Value<int>("Id");
                return id;
            }

            return -1;
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

        public bool Update(int id, string payload)
        {
            // Update Code
            var client = new WexflowServiceClient(_configs.WexflowWebServiceUri);
            var updated = client.UpdateWorkflow(_configs.Username, _configs.Password, payload);
            return updated;
        }

        public bool Delete(int id)
        {
            // Delete a workflow
            var client = new WexflowServiceClient(_configs.WexflowWebServiceUri);
            var deleted = client.DeleteWorkflow(_configs.Username, _configs.Password, id);
            return deleted;
        }
    }
}
