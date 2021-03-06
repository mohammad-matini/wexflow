﻿using Newtonsoft.Json;
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
            UpdateRecordUrl = Workflow.UpdateRecordUrl;
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
                InfoFormat("Payload: {0}", JsonConvert.SerializeObject(result));

                // Retrieve recordId
                var recordId = string.Empty;
                var recordIdSourceObj = JObject.Parse(RecordIdSource);
                var recordIdSourceType = recordIdSourceObj.Value<string>("type");
                if (recordIdSourceType.ToLower() == "static")
                {
                    recordId = recordIdSourceObj.Value<string>("id");

                    if (string.IsNullOrEmpty(recordId))
                    {
                        Info("RecordId not found.");
                    }
                    else
                    {
                        InfoFormat("RecordId: {0}", recordId);
                    }

                    // Update record from result
                    success &= UpdateRecord(recordId, result);
                }
                else
                {
                    var linkedFieldId = recordIdSourceObj.Value<string>("linkedFieldId");

                    foreach (var kvp in trigger.Payload)
                    {
                        if (kvp.Key == linkedFieldId)
                        {
                            if (linkedFieldId == "_id")
                            {
                                var rId = kvp.Value.ToString();
                                success &= UpdateRecord(rId, result);
                            }
                            else {
                                var linkedField = JArray.Parse(kvp.Value.ToString());
                                foreach (var recordField in linkedField)
                                {
                                    var rId = recordField.Value<string>("_id");
                                    success &= UpdateRecord(rId, result);
                                }
                            }
                            
                            break;
                        }
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

        private bool UpdateRecord(string recordId, Dictionary<string, object> result)
        {
            if (result.Count > 0)
            {
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
                    return true;
                }
                else
                {
                    ErrorFormat("An error occured while updating the record {0}: {1}", recordId, response);
                    return false;
                }
            }
            else
            {
                Info("The mapping resulted in an empty payload.");
            }

            return false;
        }

    }
}
