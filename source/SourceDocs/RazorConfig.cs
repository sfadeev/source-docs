using System.Collections.Generic;
using Nancy.ViewEngines.Razor;

namespace SourceDocs
{
    public class RazorConfig : IRazorConfiguration
    {
        public IEnumerable<string> GetAssemblyNames()
        {
            yield return typeof(System.Web.HttpApplication).Assembly.FullName;
        }

        public IEnumerable<string> GetDefaultNamespaces()
        {
            yield break;
        }

        public bool AutoIncludeModelNamespace
        {
            get { return true; }
        }
    }
}