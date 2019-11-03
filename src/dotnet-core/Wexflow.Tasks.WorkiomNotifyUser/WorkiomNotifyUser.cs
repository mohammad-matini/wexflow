using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Xml.Linq;
using Wexflow.Core;
using Workiom.Core;
using System.Linq;
using Albatross.Expression;

namespace Wexflow.Tasks.WorkiomNotifyUser
{
    public class WorkiomNotifyUser : Task
    {
        public string NotifyUserUrl { get; }
        public string Mapping { get; }

        public WorkiomNotifyUser(XElement xe, Workflow wf) : base(xe, wf)
        {
            //NotifyUserUrl = GetSetting("notifyUserUrl");
            NotifyUserUrl = Workflow.NotifyUserUrl;
            Mapping = GetSetting("mapping");
        }

        public override TaskStatus Run()
        {
            Info("Notifying Workiom user...");

            bool success = true;

            try
            {
                //var userId = int.Parse(Workflow.RestParams["UserId"]);

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

                if (result.Count > 0)
                {
                    var userId = result.Values.First();
                    var message = Workflow.RestParams["Message"];

                    //var parser = Factory.Instance.Create();
                    //var finalMessage = parser.Compile(message).EvalText("");

                    //DataRowExecutionContextFactory factory = new DataRowExecutionContextFactory(Factory.Instance.Create());
                    //IExecutionContext<System.Data.DataRow> context = factory.Create();
                    //context.SetExpression("message", message);
                    //System.Data.DataTable table = new System.Data.DataTable();
                    //table.Columns.Add("message", typeof(string));
                    //table.Rows.Add(message);
                    //var finalMessage = context.GetValue("message", table.Rows[0]);

                    var json = "{\"userId\":" + userId + ",\"message\":\"" + message + "\"}";
                    InfoFormat("Payload: {0}", json);
                    var auth = Workflow.GetWorkiomAccessToken();
                    var notifyTask = WorkiomHelper.Post(NotifyUserUrl, auth, json);
                    notifyTask.Wait();

                    var response = notifyTask.Result;
                    var responseSuccess = (bool)JObject.Parse(response).SelectToken("success");

                    if (responseSuccess)
                    {
                        InfoFormat("User {0} notified.", userId);
                    }
                    else
                    {
                        ErrorFormat("An error occured while notifying the user {0}: {1}", userId, response);
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
                ErrorFormat("An error occured while notifying Workiom user. Error: {0}", e.Message);
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
