using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Workiom.Core
{
    public class WorkiomHelper
    {
        public static Dictionary<string, object> Map(Trigger trigger, Dictionary<string, MappingValue> mapping)
        {
            var result = new Dictionary<string, object>();

            foreach (var item in mapping)
            {
                if (item.Value.MappingType == MappingType.Dynamic && trigger.Payload.ContainsKey(item.Value.Value))
                {
                    result[item.Key] = trigger.Payload[item.Value.Value];
                }
                else if (item.Value.MappingType == MappingType.Static)
                {
                    result[item.Key] = item.Value.Value;
                }
            }

            return result;
        }

        public static async Task<string> Post(string url, string auth, string json)
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

        public static async Task<string> Put(string url, string auth, string json)
        {
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth);
                var httpResponse = await httpClient.PutAsync(url, httpContent);
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
