using Newtonsoft.Json.Linq;
using System;
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
        private static readonly string MappingKey = "Mapping";

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
                if (!Workflow.RestParams.ContainsKey(MappingKey))
                {
                    Error(MappingKey + " key not found in REST params.");
                    success = false;
                }
                else
                {
                    // Parse JSON
                    var json = Workflow.RestParams[MappingKey];
                    var o = JObject.Parse(json);
                    //var auth = (string)o.SelectToken("Authorization");
                    var auth = Workflow.GetWorkiomAccessToken();
                    var listId = (string)o.SelectToken("listId");
                    var payload = o.SelectToken("Payload").ToString();

                    // REST call
                    var url = CreateRecordUrl + listId;
                    var responseTask = Post(url, auth, payload);
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
