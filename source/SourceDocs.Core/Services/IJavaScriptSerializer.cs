using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SourceDocs.Core.Services
{
    public interface IJavaScriptSerializer
    {
        string Serialize(object obj);

        T Deserialize<T>(string input);
    }

    public class DefaultJavaScriptSerializer : IJavaScriptSerializer
    {
        public string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.Indented,
                new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });
        }

        public T Deserialize<T>(string input)
        {
            return JsonConvert.DeserializeObject<T>(input);
        }
    }
}
