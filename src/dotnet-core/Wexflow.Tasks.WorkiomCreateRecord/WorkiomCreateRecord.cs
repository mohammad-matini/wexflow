using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Xml.Linq;
using Wexflow.Core;
using Workiom.Core;

namespace Wexflow.Tasks.WorkiomCreateRecord
{
    public class WorkiomCreateRecord : Task
    {
        public string CreateRecordUrl { get; }
        public string ListId { get; }
        public string Mapping { get; }

        public WorkiomCreateRecord(XElement xe, Workflow wf) : base(xe, wf)
        {
            CreateRecordUrl = GetSetting("createRecordUrl");
            ListId = GetSetting("listId");
            Mapping = GetSetting("mapping");
        }

        public override TaskStatus Run()
        {
            Info("Creating Workiom record ...");

            //Thread.Sleep(10 * 1000); // To test queuing

            bool success = true;

            try
            {
                InfoFormat("Mapping: {0}", Mapping);

                // Retrieve payload
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

                // Create record from result
                if (result.Count > 0)
                {
                    var url = CreateRecordUrl + ListId;
                    var auth = Workflow.GetWorkiomAccessToken();
                    var json = JsonConvert.SerializeObject(result);
                    InfoFormat("Payload: {0}", json);

                    var createTask = WorkiomHelper.Post(url, auth, json);
                    createTask.Wait();
                    var response = createTask.Result;
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



    }
}
