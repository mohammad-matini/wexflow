using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Xml.Linq;
using Wexflow.Core;
using Workiom.Core;
using System.Threading;
using System.Net.Mail;
using System.Net;

namespace Wexflow.Tasks.WorkiomMailsSender
{
    public class WorkiomMailsSender : Task
    {
        public string Host { get; }
        public int Port { get; }
        public bool EnableSsl { get; }
        public string User { get; }
        public string Password { get; }
        public bool IsBodyHtml { get; }
        public string From { get; }
        public string ToMapping { get; }
        public string CcMapping { get; }
        public string BccMapping { get; }
        public string SubjectMapping { get; }
        public string BodyMapping { get; }

        public WorkiomMailsSender(XElement xe, Workflow wf) : base(xe, wf)
        {
            Host = GetSetting("host");
            Port = int.Parse(GetSetting("port"));
            EnableSsl = bool.Parse(GetSetting("enableSsl"));
            User = GetSetting("user");
            Password = GetSetting("password");
            IsBodyHtml = bool.Parse(GetSetting("isBodyHtml", "true"));
            From = GetSetting("from");
            ToMapping = GetSetting("toMapping");
            CcMapping = GetSetting("ccMapping");
            BccMapping = GetSetting("bccMapping");
            SubjectMapping = GetSetting("subjectMapping");
            BodyMapping = GetSetting("bodyMapping");
        }

        public override TaskStatus Run()
        {
            Info("Sending email ...");

            bool success = true;

            try
            {
                var to = string.Empty;
                var cc = string.Empty;
                var bcc = string.Empty;
                var subject = string.Empty;
                var body = string.Empty;

                var trigger = new Trigger { Payload = JsonConvert.DeserializeObject<Dictionary<string, object>>(Workflow.RestParams["Payload"]) };

                // toMapping
                InfoFormat("toMapping: {0}", ToMapping);
                var jArray = JArray.Parse(ToMapping);
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
                    to = result.Values.First().ToString();
                    InfoFormat("to: {0}", to);
                }
                else
                {
                    Info("toMapping: The mapping resulted in an empty payload.");
                }

                // ccMapping
                InfoFormat("ccMapping: {0}", CcMapping);
                jArray = JArray.Parse(CcMapping);
                mapping = new Dictionary<string, MappingValue>();

                foreach (var item in jArray)
                {
                    var field = item.Value<string>("Field");
                    var val = item.Value<object>("Value");
                    var type = item.Value<string>("Type");

                    mapping.Add(field, new MappingValue { Value = val, MappingType = type.ToLower() == "field" ? MappingType.Dynamic : MappingType.Static });
                }

                // Genereate result
                result = WorkiomHelper.Map(trigger, mapping);

                if (result.Count > 0)
                {
                    cc = result.Values.First().ToString();
                    InfoFormat("cc: {0}", cc);
                }
                else
                {
                    Info("ccMapping: The mapping resulted in an empty payload.");
                }

                // bccMapping
                InfoFormat("BccMapping: {0}", BccMapping);
                jArray = JArray.Parse(BccMapping);
                mapping = new Dictionary<string, MappingValue>();

                foreach (var item in jArray)
                {
                    var field = item.Value<string>("Field");
                    var val = item.Value<object>("Value");
                    var type = item.Value<string>("Type");

                    mapping.Add(field, new MappingValue { Value = val, MappingType = type.ToLower() == "field" ? MappingType.Dynamic : MappingType.Static });
                }

                // Genereate result
                result = WorkiomHelper.Map(trigger, mapping);

                if (result.Count > 0)
                {
                    bcc = result.Values.First().ToString();
                    InfoFormat("bcc: {0}", bcc);
                }
                else
                {
                    Info("bccMapping: The mapping resulted in an empty payload.");
                }

                // subject
                InfoFormat("subjectMapping: {0}", SubjectMapping);
                jArray = JArray.Parse(SubjectMapping);
                mapping = new Dictionary<string, MappingValue>();

                foreach (var item in jArray)
                {
                    var field = item.Value<string>("Field");
                    var val = item.Value<object>("Value");
                    var type = item.Value<string>("Type");

                    mapping.Add(field, new MappingValue { Value = val, MappingType = type.ToLower() == "field" ? MappingType.Dynamic : MappingType.Static });
                }

                // Genereate result
                result = WorkiomHelper.Map(trigger, mapping);

                if (result.Count > 0)
                {
                    subject = result.Values.First().ToString();
                    InfoFormat("subject: {0}", subject);
                }
                else
                {
                    Info("subjectMapping: The mapping resulted in an empty payload.");
                }

                // body
                InfoFormat("bodyMapping: {0}", BodyMapping);
                jArray = JArray.Parse(BodyMapping);
                mapping = new Dictionary<string, MappingValue>();

                foreach (var item in jArray)
                {
                    var field = item.Value<string>("Field");
                    var val = item.Value<object>("Value");
                    var type = item.Value<string>("Type");

                    mapping.Add(field, new MappingValue { Value = val, MappingType = type.ToLower() == "field" ? MappingType.Dynamic : MappingType.Static });
                }

                // Genereate result
                result = WorkiomHelper.Map(trigger, mapping);

                if (result.Count > 0)
                {
                    body = result.Values.First().ToString();
                    InfoFormat("body: {0}", body);
                }
                else
                {
                    Info("bodyMapping: The mapping resulted in an empty payload.");
                }

                // Send email
                var tos = to.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).Where(s => s != string.Empty).ToArray();
                var ccs = cc.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).Where(s => s != string.Empty).ToArray();
                var bccs = bcc.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).Where(s => s != string.Empty).ToArray();
                Send(Host, Port, EnableSsl, User, Password, IsBodyHtml, tos, ccs, bccs, subject, body);
                Info("Mail sent.");
            }
            catch (ThreadAbortException)
            {
                throw;
            }
            catch (Exception e)
            {
                ErrorFormat("An error occured while sending email. Error: {0}", e.Message);
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

        public void Send(string host, int port, bool enableSsl, string user, string password, bool isBodyHtml, string[] tos, string[] ccs, string[] bccs, string subject, string body)
        {
            var smtp = new SmtpClient
            {
                Host = host,
                Port = port,
                EnableSsl = enableSsl,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(user, password)
            };

            using (var msg = new MailMessage())
            {
                msg.From = new MailAddress(From);
                foreach (string to in tos) msg.To.Add(new MailAddress(to));
                foreach (string cc in ccs) msg.CC.Add(new MailAddress(cc));
                foreach (string bcc in bccs) msg.Bcc.Add(new MailAddress(bcc));
                msg.Subject = subject;
                msg.Body = body;
                msg.IsBodyHtml = isBodyHtml;

                smtp.Send(msg);
            }
        }

    }
}
