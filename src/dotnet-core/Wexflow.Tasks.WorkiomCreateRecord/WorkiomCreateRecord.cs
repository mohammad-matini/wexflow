using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using Wexflow.Core;

namespace Wexflow.Tasks.WorkiomCreateRecord
{
    public class WorkiomCreateRecord : Task
    {
        public string CreateRecordUrl { get; }

        public WorkiomCreateRecord(XElement xe, Workflow wf) : base(xe, wf)
        {
            CreateRecordUrl = GetSetting("createRecordUrl");
        }

        public override TaskStatus Run()
        {
            Info("Creating Workiom record ...");

            //Thread.Sleep(10 * 1000); // To test queuing

            bool success = true;

            try
            {
                // Retrieve listId
                var listId = Workflow.RestParams["ListId"];

                // Retrieve trigger
                var trigger = new Trigger { Payload = JsonConvert.DeserializeObject<Dictionary<string, string>>(Workflow.RestParams["Trigger"]) };

                // Retrieve mapping (only dynamic for the moment)
                var mappingDic = JsonConvert.DeserializeObject<Dictionary<string, string>>(Workflow.RestParams["Mapping"]);
                var mapping = new Dictionary<string, MappingValue>();
                foreach (var item in mappingDic)
                {
                    mapping.Add(item.Key, new MappingValue { MappingType = MappingType.Dynamic, Value = item.Value });
                }

                // Genereate result
                var result = new Dictionary<string, string>();

                foreach (var item in mapping)
                {
                    if (item.Value.MappingType == MappingType.Dynamic &&  trigger.Payload.ContainsKey(item.Value.Value))
                    {
                        result[item.Key] = trigger.Payload[item.Value.Value];
                    }
                    else
                    {
                        result[item.Key] = item.Value.Value;
                    }
                }

                // Create record from result
                var url = CreateRecordUrl + listId;
                var auth = Workflow.GetWorkiomAccessToken();
                var json = JsonConvert.SerializeObject(result);

                var responseTask = Post(url, auth, json);
                responseTask.Wait();
                var response = responseTask.Result;
                var responseSuccess = (bool)JObject.Parse(response).SelectToken("success");

                if (responseSuccess)
                {
                    Info("Record created.");
                }
                else
                {
                    ErrorFormat("An error occured while creating the record: {0}", response);
                    success = false;
                }

            }
            catch (ThreadAbortException)
            {
                throw;
            }
            catch (Exception e)
            {
                ErrorFormat("An error occured while creating Workiom record. Error: {0}", e.Message);
                success = false;
            }

            var status = Status.Success;

            if (!success)
            {
                status = Status.Error;
            }

            Info("Task finished.");
            return new TaskStatus(status);
        }

        private async System.Threading.Tasks.Task<string> Post(string url, string auth, string json)
        {
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth);
                var httpResponse = await httpClient.PostAsync(url, httpContent);
                if (httpResponse.Content != null)
                {
                    var responseContent = await httpResponse.Content.ReadAsStringAsync();
                    return responseContent;
                }
            }
            return string.Empty;
        }

    }
}
