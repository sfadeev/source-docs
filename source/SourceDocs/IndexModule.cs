using Nancy;

namespace SourceDocs
{
    public class IndexModule : NancyModule
    {
        public IndexModule()
        {
            Get["/"] = parameters =>
            {
                return View["Dashboard"];
            };
        }
    }
}