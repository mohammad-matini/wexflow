using Newtonsoft.Json.Linq;
using System;
using System.Threading;
using System.Xml.Linq;
using Wexflow.Core;
using Workiom.Core;

namespace Wexflow.Tasks.WorkiomNotifyUser
{
    public class WorkiomNotifyUser : Task
    {
        public string NotifyUserUrl { get; }

        public WorkiomNotifyUser(XElement xe, Workflow wf) : base(xe, wf)
        {
            NotifyUserUrl = GetSetting("notifyUserUrl");
        }

        public override TaskStatus Run()
        {
            Info("Notifying Workiom user...");

            bool success = true;

            try
            {
                var userId = int.Parse(Workflow.RestParams["UserId"]);
                var message = Workflow.RestParams["Message"];

                var json = "{\"userId\":" + userId + ",\"message\":\"" + message + "\"}";
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
