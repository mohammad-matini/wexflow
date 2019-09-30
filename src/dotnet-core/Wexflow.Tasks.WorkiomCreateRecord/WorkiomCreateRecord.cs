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
        public string[] Mappings { get; }

        public WorkiomCreateRecord(XElement xe, Workflow wf) : base(xe, wf)
        {
            CreateRecordUrl = GetSetting("createRecordUrl");
            ListId = GetSetting("listId");
            Mappings = GetSettings("mapping");
        }

        public override TaskStatus Run()
        {
            Info("Creating Workiom record ...");

            //Thread.Sleep(10 * 1000); // To test queuing

            bool success = true;

            try
            {
                // Retrieve payload
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

                    // Create record from result
                    var url = CreateRecordUrl + ListId;
                    var auth = Workflow.GetWorkiomAccessToken();
                    var json = JsonConvert.SerializeObject(result);

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
