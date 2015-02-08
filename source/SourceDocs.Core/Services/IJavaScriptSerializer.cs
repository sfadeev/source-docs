using Newtonsoft.Json;

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
            return JsonConvert.SerializeObject(obj);
        }

        public T Deserialize<T>(string input)
        {
            return JsonConvert.DeserializeObject<T>(input);
        }
    }
}
