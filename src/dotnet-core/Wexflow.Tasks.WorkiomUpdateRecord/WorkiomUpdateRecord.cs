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
        public string Mapping { get; }
        public string RecordIdSource { get; }

        public WorkiomUpdateRecord(XElement xe, Workflow wf) : base(xe, wf)
        {
            UpdateRecordUrl = GetSetting("updateRecordUrl");
            ListId = GetSetting("listId");
            Mapping = GetSetting("mapping");
            RecordIdSource = GetSetting("recordIdSource");
        }

        public override TaskStatus Run()
        {
            Info("Updating Workiom record ...");

            //Thread.Sleep(10 * 1000); // To test queuing

            bool success = true;

            try
            {
                InfoFormat("Mapping: {0}", Mapping);

                // Retrieve trigger
                var trigger = new Trigger { Payload = JsonConvert.DeserializeObject<Dictionary<string, object>>(Workflow.RestParams["Payload"]) };

                // Retrieve recordId
                //var recordId = Workflow.RestParams["RecordId"];
                var recordId = string.Empty;
                var recordIdSourceObj = JObject.Parse(RecordIdSource);
                var recordIdSourceType = recordIdSourceObj.Value<string>("type");
                if (recordIdSourceType.ToLower() == "static")
                {
                    recordId = recordIdSourceObj.Value<string>("id");
                }
                else
                {
                    var linkedFieledId = recordIdSourceObj.Value<string>("linkedFieledId");

                    foreach (var kvp in trigger.Payload)
                    {
                        if (kvp.Key == linkedFieledId)
                        {
                            var linkedFiled = JArray.Parse(kvp.Value.ToString());
                            recordId = linkedFiled[0].Value<string>();
                            break;
                        }
                    }

                }

                if (string.IsNullOrEmpty(recordId))
                {
                    Info("RecordId not found.");
                }
                else
                {
                    InfoFormat("RecordId: {0}", recordId);
                }

                // Retrieve mapping
                var jArray = JArray.Parse(Mapping);
                var mapping = new Dictionary<string, MappingValue>();

                foreach (var item in jArray)
                {
                    var field = item.Value<string>("Field");
                    var val = item.Value<object>("Value");
                    var type = item.Value<string>("Type");

                    mapping.Add(field, new MappingValue { Value = val, MappingType = type.ToLower() == "field" ? MappingType.Dynamic : MappingType.Static });
                }

                // Genereate result
                var result = WorkiomHelper.Map(trigger, mapping);

                // Update record from result
                if (result.Count > 0)
                {
                    var url = UpdateRecordUrl + ListId + "&id=" + recordId;
                    var auth = Workflow.GetWorkiomAccessToken();
                    var json = JsonConvert.SerializeObject(result);
                    InfoFormat("Payload: {0}", json);

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
                else
                {
                    Info("The mapping resulted in an empty payload.");
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
