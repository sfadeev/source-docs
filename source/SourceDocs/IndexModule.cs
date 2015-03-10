using Nancy;

namespace SourceDocs
{
    public class IndexModule : NancyModule
    {
        public IndexModule()
        {
            Get[""] = parameters =>
            {
                return View["index2"];
            };

            Get["/test"] = x =>
            {
                return View["test"];
            };

            Get["/Dashboard{index}"] = x =>
            {
                return View["Dashboard" + x.index];
            };

            Get["{path*}"] = parameters =>
            {
                return View["index"];
            };
        }
    }
}