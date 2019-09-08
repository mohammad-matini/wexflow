using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Threading;
using System.Xml.Linq;
using Wexflow.Core;

namespace Wexflow.Tasks.WorkiomCreateRecord
{
    public class WorkiomCreateRecord : Task
    {

        public string MappingFile { get; }

        public WorkiomCreateRecord(XElement xe, Workflow wf) : base(xe, wf)
        {
            MappingFile = GetSetting("mappingFile");
        }

        public override TaskStatus Run()
        {
            Info("Creating Workiom record ...");

            bool success = true;

            try
            {
                // TODO parse JSON
                string json = File.ReadAllText(MappingFile);
                JObject o = JObject.Parse(json);
                // TODO REST call

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
