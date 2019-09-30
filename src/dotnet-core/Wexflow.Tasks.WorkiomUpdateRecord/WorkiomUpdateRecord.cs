using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Xml.Linq;
using Wexflow.Core;
using Workiom.Core;

namespace Wexflow.Tasks.WorkiomUpdateRecord
{
    public class WorkiomUpdateRecord : Task
    {
        public string UpdateRecordUrl { get; }
        public string ListId { get; }
        public string[] Mappings { get; }

        public WorkiomUpdateRecord(XElement xe, Workflow wf) : base(xe, wf)
        {
            UpdateRecordUrl = GetSetting("updateRecordUrl");
            ListId = GetSetting("listId");
            Mappings = GetSettings("mapping");
        }

        public override TaskStatus Run()
        {
            Info("Updating Workiom record ...");

            //Thread.Sleep(10 * 1000); // To test queuing

            bool success = true;

            try
            {
                // Retrieve recordId
                var recordId = Workflow.RestParams["RecordId"];

                // Retrieve trigger
                var trigger = new Trigger { Payload = JsonConvert.DeserializeObject<Dictionary<string, string>>(Workflow.RestParams["Payload"]) };

                // Retrieve mapping (only dynamic for the moment)
                foreach (var map in Mappings)
                {
                    var mappingDic = JsonConvert.DeserializeObject<Dictionary<string, string>>(map);
                    var mapping = new Dictionary<string, MappingValue>();
                    foreach (var item in mappingDic)
                    {
                        mapping.Add(item.Key, new MappingValue { MappingType = MappingType.Dynamic, Value = item.Value });
                    }

                    // Genereate result
                    var result = WorkiomHelper.Map(trigger, mapping);

                    // Update record from result
                    var url = UpdateRecordUrl + ListId + "&id=" + recordId;
                    var auth = Workflow.GetWorkiomAccessToken();
                    var json = JsonConvert.SerializeObject(result);

                    var updateTask = WorkiomHelper.Put(url, auth, json);
                    updateTask.Wait();
                    var response = updateTask.Result;
                    var responseSuccess = (bool)JObject.Parse(response).SelectToken("success");

                    if (responseSuccess)
                    {
                        Info("Record " + recordId + " updated.");
                    }
                    else
                    {
                        ErrorFormat("An error occured while updating the record {0}: {1}", recordId, response);
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
                ErrorFormat("An error occured while updating Workiom record. Error: {0}", e.Message);
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

    }
}
