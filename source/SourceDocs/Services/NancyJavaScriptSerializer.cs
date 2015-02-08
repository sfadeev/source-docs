using Nancy.Json;
using SourceDocs.Core.Services;

namespace SourceDocs.Services
{
    public class NancyJavaScriptSerializer : IJavaScriptSerializer
    {
        private readonly JavaScriptSerializer _serializer = new JavaScriptSerializer();

        public string Serialize(object obj)
        {
            return _serializer.Serialize(obj);
        }

        public T Deserialize<T>(string input)
        {
            return _serializer.Deserialize<T>(input);
        }
    }
}