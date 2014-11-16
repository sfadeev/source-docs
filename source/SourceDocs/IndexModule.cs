using Nancy;

namespace SourceDocs
{
    public class IndexModule : NancyModule
    {
        public IndexModule()
        {
            Get["/"] = parameters =>
            {
                return View["index"];
            };

            Get["/dashboard1"] = parameters =>
            {
                return View["Dashboard1"];
            };

            Get["/dashboard2"] = parameters =>
            {
                return View["Dashboard2"];
            };
        }
    }
}