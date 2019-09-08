using Newtonsoft.Json.Linq;
using System;
using System.IO;
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
        public string MappingFile { get; }

        public WorkiomCreateRecord(XElement xe, Workflow wf) : base(xe, wf)
        {
            CreateRecordUrl = GetSetting("createRecordUrl");
            MappingFile = GetSetting("mappingFile");
        }

        public override TaskStatus Run()
        {
            Info("Creating Workiom record ...");

            bool success = true;

            try
            {
                // Parse JSON
                string json = File.ReadAllText(MappingFile);
                var o = JObject.Parse(json);
                var auth = (string)o.SelectToken("Autorization");
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
