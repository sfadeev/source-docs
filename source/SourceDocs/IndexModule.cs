using Nancy;

namespace SourceDocs
{
    public class IndexModule : NancyModule
    {
        public IndexModule()
        {
            Get[""] = parameters =>
            {
                return View["index"];
            };
            
            Get["{path*}"] = parameters =>
            {
                return View["index"];
            };

            Get["/Dashboard{index}"] = x =>
            {
                return View["Dashboard" + x.index];
            };
        }
    }
}